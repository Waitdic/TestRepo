namespace iVectorOne.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using iVectorOne.Models.Transfer;
    using Microsoft.Extensions.Logging;

    public class TransferSupplierLogRepository : ITransferSupplierLogRepository
    {
        private readonly ISql _sql;
        private readonly ILogger<TransferSupplierLogRepository> _logger;

        public TransferSupplierLogRepository(ISqlFactory sqlFactory, ILogger<TransferSupplierLogRepository> logger)
        {
            _sql = Ensure.IsNotNull(sqlFactory, nameof(sqlFactory)).CreateSqlContext("Telemetry");
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public Task LogPrebookRequestsAsync(TransferDetails transferDetails)
            => LogSupplierCall(transferDetails, LogType.Prebook);

        public Task LogBookRequestsAsync(TransferDetails transferDetails)
            => LogSupplierCall(transferDetails, LogType.Book);

        public Task LogPrecancelRequestsAsync(TransferDetails transferDetails)
            => LogSupplierCall(transferDetails, LogType.Precancel);

        public Task LogCancelRequestsAsync(TransferDetails transferDetails)
            => LogSupplierCall(transferDetails, LogType.Cancel);

        private async Task LogSupplierCall(TransferDetails transferDetails, LogType logType)
        {
            try
            {
                foreach (var log in transferDetails.SupplierLogs)
                {
                    await _sql.ExecuteAsync(
                    @"INSERT INTO [TransferSupplierAPILog] (
                            [AccountID],
                            [SupplierID],
                            [Type],
                            [Title],
                            [RequestDateTime],
                            [ResponseTime],
                            [Successful],
                            [RequestLog],
                            [ResponseLog],
                            [BookingID]
                        ) VALUES (
                            @accountId,
                            @supplierId,
                            @type,
                            @title,
                            @requestDateTime,
                            @responseTime,
                            @successful,
                            @requestLog,
                            @responseLog,
                            @bookingId)",
                    new CommandSettings()
                        .WithParameters(new
                        {
                            accountId = transferDetails.AccountID,
                            supplierId = transferDetails.SupplierID,
                            type = logType.ToString(),
                            title = log.Title,
                            requestDateTime = log.Request.RequestDateTime,
                            responseTime = log.Request.RequestDuration * 1000,
                            successful = log.Request.Success,
                            requestLog = log.Request.RequestLog,
                            responseLog = log.Request.ResponseLog,
                            bookingId = transferDetails.TransferBookingID
                        }));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transfer Supplier Prebook log exception");
            }
        }
    }
}