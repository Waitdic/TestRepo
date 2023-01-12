namespace iVectorOne.Factories
{
    using Intuitive;
    using iVectorOne.Constants;
    using iVectorOne.Repositories;
    using iVectorOne.SDK.V2.ExtraBook;
    using iVectorOne.Services;
    using iVectorOne.Models.Tokens;
    using iVectorOne.Models.Extra;

    /// <summary>
    /// A factory that creates extra book responses using the provided extra details
    /// </summary>
    /// <seealso cref="ThirdParty.Service.Factories.IExtraBookResponseFactory" />
    public class ExtraBookResponseFactory : IExtraBookResponseFactory
    {
        /// <summary>Initializes a new instance of the <see cref="ExtraBookResponseFactory" /> class.</summary>
        /// <param name="tokenService">The token service, that encodes and decodes response and request tokens.</param>
        public ExtraBookResponseFactory()
        {
        }

        /// <summary>
        /// Creates a book response using information from the extra details
        /// </summary>
        /// <param name="extraDetails">The extra details which contain all the information from the third party pre book</param>
        /// <returns>A book response</returns>
        public Response Create(ExtraDetails extraDetails)
        {
            var response = new Response()
            {
                SupplierBookingReference = extraDetails.ConfirmationReference,
                SupplierReference = extraDetails.SupplierReference,
            };

            return response;
        }
    }
}
