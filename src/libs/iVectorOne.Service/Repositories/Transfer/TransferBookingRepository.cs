namespace iVectorOne.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Transfer;
    using Microsoft.Extensions.Logging;

    public class TransferBookingRepository : ITransferBookingRepository
    {
        private readonly ISql _sql;
        private readonly ICurrencyLookupRepository _currencyRepository;
        private readonly ITPSupport _support;
        private readonly ILogger<TransferBookingRepository> _logger;

        public TransferBookingRepository(
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

        public async Task<int> StoreBookingAsync(TransferDetails transferDetails, bool requestValid, bool success)
        {
            int bookingId = 0;
            try
            {
                var isoCurrencyId = await _support.ISOCurrencyIDLookupAsync(transferDetails.ISOCurrencyCode);
                var exchangeRate = await _currencyRepository.GetExchangeRateFromISOCurrencyIDAsync(isoCurrencyId);
                var status = !requestValid ? BookingStatus.Invalid : (success ? BookingStatus.Live : BookingStatus.Failed);

                bookingId = await _sql.ReadScalarAsync<int>(
                    "TransferBooking_Upsert",
                    new CommandSettings()
                        .IsStoredProcedure()
                        .WithParameters(new
                        {
                            bookingReference = transferDetails.BookingReference,
                            supplierBookingReference = transferDetails.ConfirmationReference,
                            accountId = transferDetails.AccountID,
                            supplierId = transferDetails.SupplierID,
                            status = status.ToString(),
                            leadGuestName = $"{transferDetails.LeadGuestTitle} {transferDetails.LeadGuestFirstName} {transferDetails.LeadGuestLastName}",
                            departureDate = transferDetails.DepartureDate,
                            totalPrice = transferDetails.LocalCost,
                            isoCurrencyId = isoCurrencyId,
                            estimatedGBPPrice = transferDetails.LocalCost * exchangeRate,
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