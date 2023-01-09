namespace iVectorOne.Repositories
{
    using System.Threading.Tasks;
    using TransferPrebook = SDK.V2.TransferPrebook;
    using TransferBook = SDK.V2.TransferBook;
    using TransferPrecancel = SDK.V2.TransferPrecancel;
    using TransferCancel = SDK.V2.TransferCancel;

    /// <summary>Defines a repository for saving book, pre book and cancel requests and responses</summary>
    public interface ITransferAPILogRepository
    {
        /// <summary>Logs the transfer pre book request and response.</summary>
        /// <param name="request">The pre book request.</param>
        /// <param name="response">The pre book response.</param>
        /// <param name="success">True if the request was process successfully</param>
        Task LogPrebookAsync(TransferPrebook.Request request, TransferPrebook.Response response, bool success);

        /// <summary>Logs the transfer book request</summary>
        /// <param name="request">The book request.</param>
        /// <param name="response">The book response.</param>
        /// <param name="success">True if the request was process successfully</param>
        Task LogBookAsync(TransferBook.Request request, TransferBook.Response response, bool success);

        /// <summary>Logs the transfer pre cancel request</summary>
        /// <param name="request">The pre cancel request.</param>
        /// <param name="response">The pre cancel response.</param>
        /// <param name="success">True if the request was process successfully</param>
        Task LogPrecancelAsync(TransferPrecancel.Request request, TransferPrecancel.Response response, bool success);

        /// <summary>Logs the transfer cancel request</summary>
        /// <param name="request">The cancel request.</param>
        /// <param name="response">The cancel response.</param>
        /// <param name="success">True if the request was process successfully</param>
        Task LogCancelAsync(TransferCancel.Request request, TransferCancel.Response response, bool success);
    }
}