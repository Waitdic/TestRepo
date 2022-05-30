namespace ThirdParty.Services
{
    using System.Threading.Tasks;
    using ThirdParty.Models;
    using ThirdParty.SDK.V2.PropertyBook;

    /// <summary>Service responsible for talking to the third party and handling the book, and returning book responses.</summary>
    public interface IBookService
    {
        /// <summary>Books the specified book request.</summary>
        /// <param name="bookRequest">The book request.</param>
        /// <param name="user">The user.</param>
        /// <returns>
        /// A book response
        /// </returns>
        Task<Response> BookAsync(Request bookRequest, User user);
    }
}