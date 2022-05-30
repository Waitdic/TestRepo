namespace ThirdParty.Factories
{
    using ThirdParty.Constants;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.Models.Tokens;
    using ThirdParty.Repositories;
    using ThirdParty.SDK.V2.PropertyBook;
    using ThirdParty.Services;

    /// <summary>
    /// A factory that creates property book responses using the provided property details
    /// </summary>
    /// <seealso cref="ThirdParty.Service.Factories.IPropertyBookResponseFactory" />
    public class PropertyBookResponseFactory : IPropertyBookResponseFactory
    {
        /// <summary>Repository for looking up Currency information.</summary>
        private readonly ICurrencyLookupRepository currencyRepository;

        /// <summary>The token service</summary>
        private ITokenService tokenService;

        /// <summary>Initializes a new instance of the <see cref="PropertyBookResponseFactory" /> class.</summary>
        /// <param name="tokenService">The token service, that encodes and decodes response and request tokens.</param>
        /// <param name="currencyRepository">Repository for looking up Currency information.</param>
        public PropertyBookResponseFactory(
            ITokenService tokenService, 
            ICurrencyLookupRepository currencyRepository)
        {
            this.tokenService = tokenService;
            this.currencyRepository = currencyRepository;
        }

        /// <summary>
        /// Creates a book response using information from the property details
        /// </summary>
        /// <param name="propertyDetails">The property details which contain all the information from the third party pre book</param>
        /// <returns>A book response</returns>
        public Response Create(PropertyDetails propertyDetails)
        {
            var token = new BookToken()
            {
                PropertyID = propertyDetails.TPPropertyID
            };

            var response = new Response()
            {
                SupplierBookingReference = propertyDetails.SupplierSourceReference,
                BookToken = this.tokenService.EncodeBookToken(token),
                SupplierReference1 = this.GetSupplierReference1(propertyDetails),
                SupplierReference2 = this.GetSupplierReference2(propertyDetails)
            };

            return response;
        }

        private string GetSupplierReference1(PropertyDetails propertyDetails)
        {
            return propertyDetails.Source switch
            {    
                ThirdParties.MTS => propertyDetails.Rooms[0].ThirdPartyReference,
                _ => propertyDetails.SourceSecondaryReference,
            };
        }

        private string GetSupplierReference2(PropertyDetails propertyDetails)
        {
            return propertyDetails.TPRef1;
        }
    }
}
