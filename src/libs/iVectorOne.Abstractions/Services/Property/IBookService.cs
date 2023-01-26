namespace iVectorOne.Services
{
    using System.Threading.Tasks;
    using iVectorOne.SDK.V2.PropertyBook;

    /// <summary>Service responsible for talking to the third party and handling the book, and returning book responses.</summary>
    public interface IBookService
    {
        /// <summary>Books the specified book request.</summary>
        /// <param name="bookRequest">The book request.</param>
        /// <returns>A book response </returns>
        Task<Response> BookAsync(Request bookRequest);
    }
}