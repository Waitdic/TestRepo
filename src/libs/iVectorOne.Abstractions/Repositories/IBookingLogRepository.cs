namespace iVectorOne.Repositories
{
    using System.Threading.Tasks;
    using Prebook = SDK.V2.PropertyPrebook;
    using Book = SDK.V2.PropertyBook;
    using Precancel = SDK.V2.PropertyPrecancel;
    using Cancel = SDK.V2.PropertyCancel;

    /// <summary>Defines a repository for saving book, pre book and cancel requests and responses</summary>
    public interface IBookingLogRepository
    {
        /// <summary>Logs the pre book request and response.</summary>
        /// <param name="request">The pre book request.</param>
        /// <param name="response">The pre book response.</param>
        /// <param name="success">True if the request was process successfully</param>
        Task LogPrebookAsync(Prebook.Request request, Prebook.Response response, bool success);

        /// <summary>Logs the book request</summary>
        /// <param name="request">The book request.</param>
        /// <param name="response">The book response.</param>
        /// <param name="success">True if the request was process successfully</param>
        Task LogBookAsync(Book.Request request, Book.Response response, bool success);

        /// <summary>Logs the pre cancel request</summary>
        /// <param name="request">The pre cancel request.</param>
        /// <param name="response">The pre cancel response.</param>
        /// <param name="success">True if the request was process successfully</param>
        Task LogPrecancelAsync(Precancel.Request request, Precancel.Response response, bool success);

        /// <summary>Logs the cancel request</summary>
        /// <param name="request">The cancel request.</param>
        /// <param name="response">The cancel response.</param>
        /// <param name="success">True if the request was process successfully</param>
        Task LogCancelAsync(Cancel.Request request, Cancel.Response response, bool success);
    }
}