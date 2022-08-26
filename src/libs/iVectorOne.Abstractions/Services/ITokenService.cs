namespace iVectorOne.Services
{
    using System.Threading.Tasks;
    using iVectorOne.Models;
    using iVectorOne.Models.Tokens;

    /// <summary>Token Service, responsible for encoding and decoding tokens</summary>
    public interface ITokenService
    {
        /// <summary>Decodes the book token.</summary>
        /// <param name="tokenString">The token string.</param>
        /// <returns>a book Token object</returns>
        Task<BookToken?> DecodeBookTokenAsync(string tokenString, Account account);

        /// <summary>Decodes the property token.</summary>
        /// <param name="tokenString">The token string.</param>
        /// <returns>a Property Token object</returns>
        Task<PropertyToken?> DecodePropertyTokenAsync(string tokenString, Account account);

        /// <summary>Decodes the room token.</summary>
        /// <param name="tokenString">The token string.</param>
        /// <returns>A room booking token object</returns>
        RoomToken? DecodeRoomToken(string tokenString);

        /// <summary>Encodes the property token.</summary>
        /// <param name="bookToken">The book token.</param>
        /// <returns>An encoded book token.</returns>
        string EncodeBookToken(BookToken bookToken);

        /// <summary>Encodes the property token.</summary>
        /// <param name="propertyToken">The property token.</param>
        /// <returns>An encoded property token.</returns>
        string EncodePropertyToken(PropertyToken propertyToken);

        /// <summary>Encodes the room token.</summary>
        /// <param name="propertyToken">The property token.</param>
        /// <returns>an encoded room token</returns>
        string EncodeRoomToken(RoomToken propertyToken);
    }
}