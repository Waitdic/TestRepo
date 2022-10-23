namespace iVectorOne.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using iVectorOne.Lookups;
    using iVectorOne.Models.Property.Booking;
    using Microsoft.Extensions.Logging;

    public class BookingRepository : IBookingRepository
    {
        private readonly ISql _sql;
        private readonly ICurrencyLookupRepository _currencyRepository;
        private readonly ITPSupport _support;
        private readonly ILogger<BookingRepository> _logger;

        public BookingRepository(
            ISqlFactory sqlFactory,
            ICurrencyLookupRepository currencyRepository,
            ITPSupport support,
            ILogger<BookingRepository> logger)
        {
            _sql = Ensure.IsNotNull(sqlFactory, nameof(sqlFactory)).CreateSqlContext("Telemetry");
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

                bookingId = await _sql.ReadScalarAsync<int>(
                    "Booking_Upsert",
                    new CommandSettings()
                        .IsStoredProcedure()
                        .WithParameters(new
                        {
                            bookingReference = propertyDetails.BookingReference,
                            supplierBookingReference = propertyDetails.SupplierSourceReference,
                            accountId = propertyDetails.AccountID,
                            supplierId = propertyDetails.SupplierID,
                            propertyId = propertyDetails.PropertyID,
                            status = status.ToString(),
                            leadGuestName = $"{propertyDetails.LeadGuestTitle} {propertyDetails.LeadGuestFirstName} {propertyDetails.LeadGuestLastName}",
                            departureDate = propertyDetails.DepartureDate,
                            duration = propertyDetails.Duration,
                            totalPrice = propertyDetails.LocalCost,
                            isoCurrencyId = isoCurrencyId,
                            estimatedGBPPrice = propertyDetails.LocalCost * exchangeRate,
                        }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Booking store exception");
            }

            return bookingId;
        }
    }
}