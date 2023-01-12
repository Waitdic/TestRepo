namespace iVectorOne.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using Intuitive.Helpers.Net;
    using iVectorOne.Models.Logging;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Search.Models;
    using Microsoft.Extensions.Logging;

    public class SupplierLogRepository : ISupplierLogRepository
    {
        private readonly ISql _sql;
        private readonly ILogger<SupplierLogRepository> _logger;

        public SupplierLogRepository(ISqlFactory sqlFactory, ILogger<SupplierLogRepository> logger)
        {
            _sql = Ensure.IsNotNull(sqlFactory, nameof(sqlFactory)).CreateSqlContext(iVectorOneInfo.TelemetryContext);
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public Task LogSearchRequestsAsync(SearchDetails searchDetails, int supplierId, List<Request> requests)
            => LogSupplierCall(searchDetails.Account.AccountID, supplierId, 0, ConvertToSupplierLogs(requests), LogType.Search);

        public Task LogPrebookRequestsAsync(PropertyDetails propertyDetails)
            => LogSupplierCall(propertyDetails, LogType.Prebook);

        public Task LogBookRequestsAsync(PropertyDetails propertyDetails)
            => LogSupplierCall(propertyDetails, LogType.Book);

        public Task LogPrecancelRequestsAsync(PropertyDetails propertyDetails)
            => LogSupplierCall(propertyDetails, LogType.Precancel);

        public Task LogCancelRequestsAsync(PropertyDetails propertyDetails)
            => LogSupplierCall(propertyDetails, LogType.Cancel);

        private Task LogSupplierCall(PropertyDetails propertyDetails, LogType logType)
            => LogSupplierCall(
                propertyDetails.AccountID,
                propertyDetails.SupplierID,
                propertyDetails.BookingID,
                propertyDetails.SupplierLogs,
                logType);

        private async Task LogSupplierCall(int accountId, int supplierId, int bookingId, IEnumerable<SupplierLog> supplierLogs, LogType logType)
        {
            try
            {
                foreach (var log in supplierLogs)
                {
                    var requestDateTime = log.Request.RequestDateTime < System.Data.SqlTypes.SqlDateTime.MinValue.Value ?
                            System.Data.SqlTypes.SqlDateTime.MinValue.Value :
                            log.Request.RequestDateTime;

                    await _sql.ExecuteAsync("SupplierAPILog_Insert",
                        new CommandSettings()
                            .IsStoredProcedure()
                            .WithParameters(new
                            {
                                accountId = accountId,
                                supplierId = supplierId,
                                type = logType.ToString(),
                                title = log.Title,
                                requestDateTime = requestDateTime,
                                responseTime = log.Request.RequestDuration * 1000,
                                successful = log.Request.Success,
                                requestLog = log.Request.RequestLog,
                                responseLog = log.Request.ResponseLog,
                                bookingId = bookingId,
                            }));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Supplier Prebook log exception");
            }
        }

        private IEnumerable<SupplierLog> ConvertToSupplierLogs(List<Request> requests)
            => requests.Select(r => new SupplierLog() { Title = r.LogFileName, Request = r });
    }
}