namespace iVectorOne.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Extra;
    using Microsoft.Extensions.Logging;

    public class ExtraBookingRepository : IExtraBookingRepository
    {
        private readonly ISql _sql;
        private readonly ICurrencyLookupRepository _currencyRepository;
        private readonly ITPSupport _support;
        private readonly ILogger<TransferBookingRepository> _logger;

        public ExtraBookingRepository(
            ISqlFactory sqlFactory,
            ICurrencyLookupRepository currencyRepository,
            ITPSupport support,
            ILogger<TransferBookingRepository> logger)
        {
            _sql = Ensure.IsNotNull(sqlFactory, nameof(sqlFactory)).CreateSqlContext("Telemetry");
            _currencyRepository = Ensure.IsNotNull(currencyRepository, nameof(currencyRepository));
            _support = Ensure.IsNotNull(support, nameof(support));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public async Task<int> StoreBookingAsync(ExtraDetails extraDetails, bool requestValid, bool success)
        {
            int bookingId = 0;
            try
            {
                var isoCurrencyId = await _support.ISOCurrencyIDLookupAsync(extraDetails.ISOCurrencyCode);
                var exchangeRate = await _currencyRepository.GetExchangeRateFromISOCurrencyIDAsync(isoCurrencyId);
                var status = !requestValid ? BookingStatus.Invalid : (success ? BookingStatus.Live : BookingStatus.Failed);

                bookingId = await _sql.ReadScalarAsync<int>(
                    "TransferBooking_Upsert",
                    new CommandSettings()
                        .IsStoredProcedure()
                        .WithParameters(new
                        {
                            bookingReference = extraDetails.BookingReference,
                            supplierBookingReference = extraDetails.ConfirmationReference,
                            accountId = extraDetails.AccountID,
                            supplierId = extraDetails.SupplierID,
                            status = status.ToString(),
                            leadGuestName = $"{extraDetails.LeadGuestTitle} {extraDetails.LeadGuestFirstName} {extraDetails.LeadGuestLastName}",
                            departureDate = extraDetails.DepartureDate,
                            totalPrice = extraDetails.LocalCost,
                            isoCurrencyId = isoCurrencyId,
                            estimatedGBPPrice = extraDetails.LocalCost * exchangeRate,
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