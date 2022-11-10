namespace iVectorOne.Repositories
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Logging;
    using iVectorOne.Models.Property.Booking;
    using Microsoft.Extensions.Logging;

    public class BookingRepository : IBookingRepository
    {
        private readonly ISql _sql;
        private readonly ICurrencyLookupRepository _currencyRepository;
        private readonly ITPSupport _support;
        private readonly ILogger<BookingRepository> _logger;

        public BookingRepository(
            ISql sql,
            ICurrencyLookupRepository currencyRepository,
            ITPSupport support,
            ILogger<BookingRepository> logger)
        {
            _sql = Ensure.IsNotNull(sql, nameof(sql));
            _currencyRepository = Ensure.IsNotNull(currencyRepository, nameof(currencyRepository));
            _support = Ensure.IsNotNull(support, nameof(support));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public async Task<int> StoreBookingAsync(PropertyDetails propertyDetails, bool requestValid, bool success)
        {
            int bookingId = 0;
            try
            {
                var isoCurrencyId = await _support.ISOCurrencyIDLookupAsync(propertyDetails.ISOCurrencyCode);
                var exchangeRate = await _currencyRepository.GetExchangeRateFromISOCurrencyIDAsync(isoCurrencyId);
                var status = !requestValid ? BookingStatus.Invalid : (success ? BookingStatus.Live : BookingStatus.Failed);
                var booking = new Booking()
                {
                    BookingReference = propertyDetails.BookingReference,
                    SupplierBookingReference = propertyDetails.SupplierSourceReference,
                    AccountID = propertyDetails.AccountID,
                    SupplierID = propertyDetails.SupplierID,
                    PropertyID = propertyDetails.PropertyID,
                    Status = status,
                    LeadGuestName = $"{propertyDetails.LeadGuestTitle} {propertyDetails.LeadGuestFirstName} {propertyDetails.LeadGuestLastName}",
                    DepartureDate = propertyDetails.DepartureDate,
                    Duration = propertyDetails.Duration,
                    TotalPrice = propertyDetails.LocalCost,
                    ISOCurrencyID = isoCurrencyId,
                    EstimatedGBPPrice = propertyDetails.LocalCost * exchangeRate,
                };

                bookingId = await StoreBookingAsync(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Booking store exception");
            }

            return bookingId;
        }

        public async Task<int> StoreBookingAsync(Booking booking)
        {
            int bookingId = 0;
            try
            {
                bookingId = await _sql.ReadScalarAsync<int>(
                    "Booking_Upsert",
                    new CommandSettings()
                        .IsStoredProcedure()
                        .WithParameters(new
                        {
                            bookingReference = booking.BookingReference,
                            supplierBookingReference = booking.SupplierBookingReference,
                            accountId = booking.AccountID,
                            supplierId = booking.SupplierID,
                            propertyId = booking.PropertyID,
                            status = booking.Status.ToString(),
                            leadGuestName = booking.LeadGuestName,
                            departureDate = booking.DepartureDate,
                            duration = booking.Duration,
                            totalPrice = booking.TotalPrice,
                            isoCurrencyId = booking.ISOCurrencyID,
                            estimatedGBPPrice = booking.EstimatedGBPPrice,
                        }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Booking store exception");
            }

            return bookingId;
        }

        public Task<Booking?> GetBookingAsync(string supplierBookingReference, Account account)
            => GetBookingAsync(string.Empty, supplierBookingReference, account);

        public Task<Booking?> GetBookingAsync(string bookingReference, string supplierBookingReference, Account account)
            => _sql.ReadSingleMappedAsync(
                "Booking_Get",
                results => MapBooking(results),
                new CommandSettings()
                    .IsStoredProcedure()
                    .WithParameters(new
                    {
                        bookingReference = bookingReference,
                        supplierBookingReference = supplierBookingReference,
                        accountId = account.AccountID
                    }));

        private async Task<Booking?> MapBooking(ISqlResults results)
        {
            var booking = await results.ReadSingleOrDefaultAsync<Booking>();

            if (booking is not null)
            {
                var apiLogs = await results.ReadAllAsync<APILog>();
                var supplierApiLogs = await results.ReadAllAsync<SupplierAPILog>();

                booking.APILogs = apiLogs.ToList();
                booking.SupplierAPILogs = supplierApiLogs.ToList();
            }

            return booking;
        }
    }
}