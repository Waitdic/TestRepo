namespace iVectorOne.Repositories
{
    using System.Threading.Tasks;
    using ExtraPrebook = SDK.V2.ExtraPrebook;
    using ExtraBook = SDK.V2.ExtraBook;
    //using ExtraPrecancel = SDK.V2.ExtraPrecancel;
    //using ExtraCancel = SDK.V2.ExtraCancel;

    /// <summary>Defines a repository for saving book, pre book and cancel requests and responses</summary>
    public interface IExtraAPILogRepository
    {
        /// <summary>Logs the extra pre book request and response.</summary>
        /// <param name="request">The pre book request.</param>
        /// <param name="response">The pre book response.</param>
        /// <param name="success">True if the request was process successfully</param>
        Task LogPrebookAsync(ExtraPrebook.Request request, ExtraPrebook.Response response, bool success);

        /// <summary>Logs the extra book request</summary>
        /// <param name="request">The book request.</param>
        /// <param name="response">The book response.</param>
        /// <param name="success">True if the request was process successfully</param>
        Task LogBookAsync(ExtraBook.Request request, ExtraBook.Response response, bool success);

        ///// <summary>Logs the transfer pre cancel request</summary>
        ///// <param name="request">The pre cancel request.</param>
        ///// <param name="response">The pre cancel response.</param>
        ///// <param name="success">True if the request was process successfully</param>
        //Task LogPrecancelAsync(ExtraPrecancel.Request request, ExtraPrecancel.Response response, bool success);

        ///// <summary>Logs the transfer cancel request</summary>
        ///// <param name="request">The cancel request.</param>
        ///// <param name="response">The cancel response.</param>
        ///// <param name="success">True if the request was process successfully</param>
        //Task LogCancelAsync(ExtraCancel.Request request, ExtraCancel.Response response, bool success);
    }
}