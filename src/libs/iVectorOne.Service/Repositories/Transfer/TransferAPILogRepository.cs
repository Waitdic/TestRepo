namespace iVectorOne.Repositories
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using iVectorOne.Models.Logging;
    using iVectorOne.SDK.V2;
    using TransferPrebook = SDK.V2.TransferPrebook;
    using TransferBook = SDK.V2.TransferBook;
    using TransferPrecancel = SDK.V2.TransferPrecancel;
    using TransferCancel = SDK.V2.TransferCancel;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// A repository responsible for logging book, pre book and cancellation logs to the database
    /// </summary>
    /// <seealso cref="ITransferAPILogRepository" />
    public class TransferAPILogRepository : ITransferAPILogRepository
    {
        /// <summary>The log writer</summary>
        private readonly ILogger<APILogRepository> _logger;

        private readonly ISql _sql;

        /// <summary>Initializes a new instance of the <see cref="TransferAPILogRepository" /> class.</summary>
        /// <param name="logger">The log writer.</param>
        public TransferAPILogRepository(ILogger<APILogRepository> logger, ISql sql)
        {
            _logger = Ensure.IsNotNull(logger, nameof(logger));
            _sql = Ensure.IsNotNull(sql, nameof(sql));
        }

        /// <inheritdoc />
        public Task LogPrebookAsync(TransferPrebook.Request request, TransferPrebook.Response response, bool success)
            => this.InsertLogsAsync(request, response, LogType.Prebook, success);

        /// <inheritdoc />
        public Task LogBookAsync(TransferBook.Request request, TransferBook.Response response, bool success)
            => this.InsertLogsAsync(request, response, LogType.Book, success);

        /// <inheritdoc />
        public Task LogPrecancelAsync(TransferPrecancel.Request request, TransferPrecancel.Response response, bool success)
            => this.InsertLogsAsync(request, response, LogType.Precancel, success);

        /// <inheritdoc />
        public Task LogCancelAsync(TransferCancel.Request request, TransferCancel.Response response, bool success)
            => this.InsertLogsAsync(request, response, LogType.Cancel, success);
    
        /// <summary>
        /// Method that actually does all the work
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        /// <param name="logType">The log type.</param>
        private async Task InsertLogsAsync(RequestBase request, ResponseBase response, LogType logType, bool success)
        {
            try
            {
                string requestLog = JsonSerializer.Serialize((object)request);
                string responseLog = JsonSerializer.Serialize((object)response);

                await _sql.ExecuteAsync(
                    "Insert into TransferAPILog (Type, Time, RequestLog, ResponseLog, AccountID, Success, TransferBookingID) " +
                        "values (@logType,@time,@requestLog,@responseLog,@accountId,@success,@transferBookingId)",
                    new CommandSettings()
                        .WithParameters(new
                        {
                            logType = logType.ToString(),
                            @time = DateTime.Now,
                            @requestLog = requestLog,
                            @responseLog = responseLog,
                            @accountId = request.Account.AccountID,
                            @success = success,
                            @transferBookingId = request.BookingID,
                        }));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"{logType} log exception");
            }
        }
    }
}