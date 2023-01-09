namespace iVectorOne.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using iVectorOne.Models.Extra;
    using iVectorOne.Models.Logging;
    using Microsoft.Extensions.Logging;

    public class ExtraSupplierLogRepository : IExtraSupplierLogRepository
    {
        private readonly ISql _sql;
        private readonly ILogger<ExtraSupplierLogRepository> _logger;

        public ExtraSupplierLogRepository(ISqlFactory sqlFactory, ILogger<ExtraSupplierLogRepository> logger)
        {
            _sql = Ensure.IsNotNull(sqlFactory, nameof(sqlFactory)).CreateSqlContext("Telemetry");
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public Task LogPrebookRequestsAsync(ExtraDetails transferDetails)
            => LogSupplierCall(transferDetails, LogType.Prebook);

        public Task LogBookRequestsAsync(ExtraDetails transferDetails)
            => LogSupplierCall(transferDetails, LogType.Book);

        public Task LogPrecancelRequestsAsync(ExtraDetails transferDetails)
            => LogSupplierCall(transferDetails, LogType.Precancel);

        public Task LogCancelRequestsAsync(ExtraDetails transferDetails)
            => LogSupplierCall(transferDetails, LogType.Cancel);

        private async Task LogSupplierCall(ExtraDetails transferDetails, LogType logType)
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
                            [TransferBookingID]
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
                            @transferBookingId)",
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
                            transferBookingId = transferDetails.TransferBookingID
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