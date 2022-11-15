namespace iVectorOne.Factories
{
    using Intuitive;
    using iVectorOne.Constants;
    using iVectorOne.Models.Transfer;
    using iVectorOne.Models.Tokens;
    using iVectorOne.Repositories;
    using iVectorOne.SDK.V2.TransferBook;
    using iVectorOne.Services;

    /// <summary>
    /// A factory that creates transfer book responses using the provided transfer details
    /// </summary>
    /// <seealso cref="ThirdParty.Service.Factories.ITransferBookResponseFactory" />
    public class TransferBookResponseFactory : ITransferBookResponseFactory
    {
        /// <summary>The token service</summary>
        private readonly ITokenService _tokenService;

        /// <summary>Initializes a new instance of the <see cref="TransferBookResponseFactory" /> class.</summary>
        /// <param name="tokenService">The token service, that encodes and decodes response and request tokens.</param>
        public TransferBookResponseFactory(ITokenService tokenService)
        {
            _tokenService = Ensure.IsNotNull(tokenService, nameof(tokenService));
        }

        /// <summary>
        /// Creates a book response using information from the transfer details
        /// </summary>
        /// <param name="transferDetails">The transfer details which contain all the information from the third party pre book</param>
        /// <returns>A book response</returns>
        public Response Create(TransferDetails transferDetails)
        {
            var token = new BookToken()
            {
                //PropertyID = transferDetails.PropertyID
            };

            var response = new Response()
            {
                //SupplierBookingReference = transferDetails.SupplierSourceReference,
                BookToken = _tokenService.EncodeBookingToken(token),
                BookingToken = _tokenService.EncodeBookingToken(token),
                //SupplierReference1 = this.GetSupplierReference1(transferDetails),
                //SupplierReference2 = this.GetSupplierReference2(transferDetails)
            };

            return response;
        }
    }
}
