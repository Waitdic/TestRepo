namespace iVectorOne.Repositories
{
    using System;
    using System.Linq;
    using Intuitive.Data;
    using Microsoft.Extensions.Logging;
    using iVectorOne.Models;
    using Prebook = SDK.V2.PropertyPrebook;
    using Book = SDK.V2.PropertyBook;
    using Cancel = SDK.V2.PropertyCancel;
    using System.Threading.Tasks;

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
            _logger = logger;
            _sql = sql;
        }

        /// <summary>Logs the pre book.</summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        /// <param name="user">The user making the request</param>
        /// <param name="exception">An exception thrown in the process, will be stored instead of the response in the logging</param>
        public async Task LogPrebookAsync(Prebook.Request request, Prebook.Response response, Subscription user, string exception = "")
        {
            object responseObject = response;
            if (response != null && response.Warnings.Any())
            {
                responseObject = response.Warnings;
            }

            await this.InsertLogsAsync(request, responseObject, LogType.Prebook, user, exception);
        }

        /// <summary>
        /// Logs the book.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        /// <param name="user">The user making the request</param>
        /// <param name="exception">An exception thrown in the process, will be stored instead of the response in the logging</param>
        public async Task LogBookAsync(Book.Request request, Book.Response response, Subscription user, string exception = "")
        {
            object responseObject = response;
            if (response != null && response.Warnings.Any())
            {
                responseObject = response.Warnings;
            }

            await this.InsertLogsAsync(request, responseObject, LogType.Book, user, exception);
        }

        /// <summary>
        /// Logs the cancel.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        /// <param name="user">The user making the request</param>
        /// <param name="exception">An exception thrown in the process, will be stored instead of the response in the logging</param>
        public async Task LogCancelAsync(Cancel.Request request, Cancel.Response response, Subscription user, string exception = "")
        {
            object responseObject = response;
            if (response != null && response.Warnings.Any())
            {
                responseObject = response.Warnings;
            }

            await this.InsertLogsAsync(request, responseObject, LogType.Cancel, user, exception);
        }

        /// <summary>
        /// Method that actually does all the work
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        /// <param name="logType">The log type.</param>
        /// <param name="user">The user making the request</param>
        /// <param name="exception">An exception thrown in the process, will be stored instead of the response in the logging</param>
        private async Task InsertLogsAsync(object request, object response, LogType logType, Subscription user, string exception = "")
        {
            try
            {
                var requestString = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                var responseString = string.Empty;

                if (string.IsNullOrEmpty(exception))
                {
                    responseString = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                }
                else
                {
                    responseString = exception;
                }

                await _sql.ExecuteAsync(
                    "Insert into APILog (Type, Time, RequestLog, ResponseLog, Login) values (@logType,@time,@requestLog,@responseLog,@login)",
                    new CommandSettings()
                        .WithParameters(new
                        {
                            logType = logType.ToString(),
                            @time = DateTime.Now,
                            @requestLog = requestString,
                            @responseLog = responseString,
                            @login = user.Login,
                        }));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"{logType} log exception");
            }
        }
    }
}
