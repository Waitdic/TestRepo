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
        /// <param name="account">The account making the request</param>
        /// <param name="supplierBookingReference">The supplier booking reference</param>
        /// <returns>a book Token object</returns>
        Task<BookToken?> DecodeBookTokenAsync(string tokenString, Account account, string supplierBookingReference);

        /// <summary>Decodes the property token.</summary>
        /// <param name="tokenString">The token string.</param>
        /// <param name="account">The account making the request</param>
        /// <returns>a Property Token object</returns>
        Task<PropertyToken?> DecodePropertyTokenAsync(string tokenString, Account account);

        /// <summary>Decodes the room token.</summary>
        /// <param name="tokenString">The token string.</param>
        /// <param name="account">The account making the request</param>
        /// <returns>A room booking token object</returns>
        RoomToken? DecodeRoomToken(string tokenString);

        /// <summary>Encodes the property token.</summary>
        /// <param name="bookToken">The book token.</param>
        /// <returns>An encoded book token.</returns>
        string EncodeBookingToken(BookToken bookToken);

        /// <summary>Encodes the property token.</summary>
        /// <param name="propertyToken">The property token.</param>
        /// <returns>An encoded property token.</returns>
        string EncodePropertyToken(PropertyToken propertyToken);

        /// <summary>Encodes the room token.</summary>
        /// <param name="propertyToken">The property token.</param>
        /// <returns>an encoded room token</returns>
        string EncodeRoomToken(RoomToken propertyToken);

        /// <summary>Decodes the property token.</summary>
        /// <param name="tokenString">The token string.</param>
        /// <param name="account">The account making the request</param>
        /// <returns>a Property Token object</returns>
        Task<TransferToken?> DecodeTransferTokenAsync(string tokenString, Account account);

        /// <summary>Encodes the transfer token.</summary>
        /// <param name="transferToken">The transfer token.</param>
        /// <returns>An encoded property token.</returns>
        string EncodeTransferToken(TransferToken transferToken);
    }
}