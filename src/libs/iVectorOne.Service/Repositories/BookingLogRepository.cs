namespace iVectorOne.Repositories
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using iVectorOne.SDK.V2;
    using Microsoft.Extensions.Logging;
    using Book = SDK.V2.PropertyBook;
    using Cancel = SDK.V2.PropertyCancel;
    using Prebook = SDK.V2.PropertyPrebook;
    using Precancel = SDK.V2.PropertyPrecancel;

    /// <summary>
    /// A repository responsible for logging book, pre book and cancellation logs to the database
    /// </summary>
    /// <seealso cref="IBookingLogRepository" />
    public class BookingLogRepository : IBookingLogRepository
    {
        /// <summary>The log writer</summary>
        private readonly ILogger<BookingLogRepository> _logger;

        private readonly ISql _sql;

        /// <summary>Initializes a new instance of the <see cref="BookingLogRepository" /> class.</summary>
        /// <param name="logger">The log writer.</param>
        public BookingLogRepository(ILogger<BookingLogRepository> logger, ISql sql)
        {
            _logger = Ensure.IsNotNull(logger, nameof(logger));
            _sql = Ensure.IsNotNull(sql, nameof(sql));
        }

        /// <inheritdoc />
        public Task LogPrebookAsync(Prebook.Request request, Prebook.Response response, bool success)
            => this.InsertLogsAsync(request, response, LogType.Prebook, success);

        /// <inheritdoc />
        public Task LogBookAsync(Book.Request request, Book.Response response, bool success)
            => this.InsertLogsAsync(request, response, LogType.Book, success);

        /// <inheritdoc />
        public Task LogPrecancelAsync(Precancel.Request request, Precancel.Response response, bool success)
            => this.InsertLogsAsync(request, response, LogType.Precancel, success);

        /// <inheritdoc />
        public Task LogCancelAsync(Cancel.Request request, Cancel.Response response, bool success)
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
                    "Insert into APILog (Type, Time, RequestLog, ResponseLog, AccountID, Success) " +
                        "values (@logType,@time,@requestLog,@responseLog,@accountId,@success)",
                    new CommandSettings()
                        .WithParameters(new
                        {
                            logType = logType.ToString(),
                            @time = DateTime.Now,
                            @requestLog = requestLog,
                            @responseLog = responseLog,
                            @accountId = request.Account.AccountID,
                            @success = success
                        }));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"{logType} log exception");
            }
        }
    }
}