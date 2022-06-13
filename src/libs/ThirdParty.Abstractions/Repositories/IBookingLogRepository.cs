namespace ThirdParty.Repositories
{
    using System.Threading.Tasks;
    using ThirdParty.Models;
    using Prebook = SDK.V2.PropertyPrebook;
    using Book = SDK.V2.PropertyBook;
    using Cancel = SDK.V2.PropertyCancel;

    /// <summary>Defines a repository for saving book, pre book and cancel requests and responses</summary>
    public interface IBookingLogRepository
    {
        /// <summary>Logs the book request</summary>
        /// <param name="request">The book request.</param>
        /// <param name="response">The book response.</param>
        /// <param name="user">The user making the request</param>
        /// <param name="exception">An exception thrown in the process, will be stored instead of the response in the logging</param>
        Task LogBookAsync(Book.Request request, Book.Response response, User user, string exception = "");

        /// <summary>Logs the cancel request</summary>
        /// <param name="request">The cancel request.</param>
        /// <param name="response">The cancel response.</param>
        /// <param name="user">The user making the request</param>
        /// <param name="exception">An exception thrown in the process, will be stored instead of the response in the logging</param>
        Task LogCancelAsync(Cancel.Request request, Cancel.Response response, User user, string exception = "");

        /// <summary>Logs the pre book request and response.</summary>
        /// <param name="request">The pre book request.</param>
        /// <param name="response">The pre book response.</param>
        /// <param name="user">The user making the request</param>
        /// <param name="exception">An exception thrown in the process, will be stored instead of the response in the logging</param>
        Task LogPrebookAsync(Prebook.Request request, Prebook.Response response, User user, string exception = "");
    }
}