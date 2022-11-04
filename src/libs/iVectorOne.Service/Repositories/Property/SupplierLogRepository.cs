namespace iVectorOne.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using iVectorOne.Models.Property.Booking;
    using Microsoft.Extensions.Logging;

    public class SupplierLogRepository : ISupplierLogRepository
    {
        private readonly ISql _sql;
        private readonly ILogger<SupplierLogRepository> _logger;

        public SupplierLogRepository(ISqlFactory sqlFactory, ILogger<SupplierLogRepository> logger)
        {
            _sql = Ensure.IsNotNull(sqlFactory, nameof(sqlFactory)).CreateSqlContext("Telemetry");
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public Task LogPrebookRequestsAsync(PropertyDetails propertyDetails)
            => LogSupplierCall(propertyDetails, LogType.Prebook);

        public Task LogBookRequestsAsync(PropertyDetails propertyDetails)
            => LogSupplierCall(propertyDetails, LogType.Book);

        public Task LogPrecancelRequestsAsync(PropertyDetails propertyDetails)
            => LogSupplierCall(propertyDetails, LogType.Precancel);

        public Task LogCancelRequestsAsync(PropertyDetails propertyDetails)
            => LogSupplierCall(propertyDetails, LogType.Cancel);

        private async Task LogSupplierCall(PropertyDetails propertyDetails, LogType logType)
        {
            try
            {
                foreach (var log in propertyDetails.SupplierLogs)
                {
                    await _sql.ExecuteAsync(
                    @"INSERT INTO [SupplierAPILog] (
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
                            accountId = propertyDetails.AccountID,
                            supplierId = propertyDetails.SupplierID,
                            type = logType.ToString(),
                            title = log.Title,
                            requestDateTime = log.Request.RequestDateTime,
                            responseTime = log.Request.RequestDuration * 1000,
                            successful = log.Request.Success,
                            requestLog = log.Request.RequestLog,
                            responseLog = log.Request.ResponseLog,
                            bookingId = propertyDetails.BookingID
                        }));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Supplier Prebook log exception");
            }
        }
    }
}