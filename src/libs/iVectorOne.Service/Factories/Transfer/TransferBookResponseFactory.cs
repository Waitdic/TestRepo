namespace iVectorOne.Factories
{
    using Intuitive;
    using iVectorOne.Constants;
    using iVectorOne.Models.Transfer;
    using iVectorOne.Models.Tokens.Transfer;
    using iVectorOne.Repositories;
    using iVectorOne.SDK.V2.TransferBook;
    using iVectorOne.Services;
    using iVectorOne.Models.Tokens;

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
            var response = new Response()
            {
                SupplierBookingReference = transferDetails.ConfirmationReference,
                SupplierReference = transferDetails.SupplierReference,
            };

            return response;
        }
    }
}
