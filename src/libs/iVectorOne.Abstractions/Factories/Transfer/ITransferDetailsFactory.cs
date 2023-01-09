namespace iVectorOne.Factories
{
    using System.Threading.Tasks;
    using iVectorOne.Models.Transfer;
    using Prebook = SDK.V2.TransferPrebook;
    using Book = SDK.V2.TransferBook;
    using Cancel = SDK.V2.TransferCancel;
    using Precancel = SDK.V2.TransferPrecancel;

    /// <summary>
    /// Factory that builds up transfer details from api requests, used to pass into the third party code
    /// </summary>
    public interface ITransferDetailsFactory
    {
        /// <summary>
        /// Creates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A transfer details object</returns>
        Task<TransferDetails> CreateAsync(Precancel.Request request);

        /// <summary>
        /// Creates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A transfer details object</returns>
        Task<TransferDetails> CreateAsync(Cancel.Request request);

        /// <summary>
        /// Creates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A transfer details object</returns>
        Task<TransferDetails> CreateAsync(Book.Request request);

        /// <summary>
        /// Creates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A transfer details object</returns>
        Task<TransferDetails> CreateAsync(Prebook.Request request);
    }
}