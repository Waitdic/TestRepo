namespace iVectorOne.Services
{
    using iVectorOne.Models.Tokens;
    using Transfers = Models.Tokens.Transfer;
    using Extras = Models.Tokens.Extra;

    /// <summary>Token Service, responsible for encoding and decoding tokens</summary>
    public interface ITokenService
    {
        /// <summary>Decodes the book token.</summary>
        /// <param name="tokenString">The token string.</param>
        /// <returns>a book Token object</returns>
        BookToken? DecodeBookToken(string tokenString);

        /// <summary>Decodes the property token.</summary>
        /// <param name="tokenString">The token string.</param>
        /// <returns>a Property Token object</returns>
        PropertyToken? DecodePropertyToken(string tokenString);

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

        /// <summary>Decodes the transfer token.</summary>
        /// <param name="tokenString">The token string.</param>
        /// <returns>a Property Token object</returns>
        Transfers.TransferToken? DecodeTransferToken(string tokenString);

        /// <summary>Encodes the transfer token.</summary>
        /// <param name="transferToken">The transfer token.</param>
        /// <returns>An encoded property token.</returns>
        string EncodeTransferToken(Transfers.TransferToken transferToken);

        /// <summary>Decodes the extra token.</summary>
        /// <param name="tokenString">The token string.</param>
        /// <returns>a Property Token object</returns>
        Extras.ExtraToken? DecodeExtraToken(string tokenString);

        /// <summary>Encodes the transfer token.</summary>
        /// <param name="transferToken">The transfer token.</param>
        /// <returns>An encoded property token.</returns>
        string EncodeExtraToken(Extras.ExtraToken extraToken);
    }
}