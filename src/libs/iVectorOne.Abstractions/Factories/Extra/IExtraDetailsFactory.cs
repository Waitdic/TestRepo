

namespace iVectorOne.Factories
{
    using iVectorOne.Models.Extra;
    using System.Threading.Tasks;
    using Prebook = SDK.V2.ExtraPrebook;
    using Book = SDK.V2.ExtraBook;

    public interface IExtraDetailsFactory
    {
        ///// <summary>
        ///// Creates the specified request.
        ///// </summary>
        ///// <param name="request">The request.</param>
        ///// <returns>A extra details object</returns>
        //Task<ExtraDetails> CreateAsync(Precancel.Request request);

        ///// <summary>
        ///// Creates the specified request.
        ///// </summary>
        ///// <param name="request">The request.</param>
        ///// <returns>A extra details object</returns>
        //Task<ExtraDetails> CreateAsync(Cancel.Request request);

        /// <summary>
        /// Creates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A extra details object</returns>
        Task<ExtraDetails> CreateAsync(Book.Request request);

        /// <summary>
        /// Creates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A extra details object</returns>
        Task<ExtraDetails> CreateAsync(Prebook.Request request);

    }
}
