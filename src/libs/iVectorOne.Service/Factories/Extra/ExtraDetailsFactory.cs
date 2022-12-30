
namespace iVectorOne.Factories
{
    using Intuitive;
    using iVectorOne.Lookups;
    using iVectorOne.Models.Extra;
    using iVectorOne.Models.Tokens.Extra;
    using iVectorOne.SDK.V2;
    using iVectorOne.Services;
    using System;
    using System.Threading.Tasks;
    using Prebook = SDK.V2.ExtraPrebook;
    using Book = SDK.V2.ExtraBook;

    public class ExtraDetailsFactory : IExtraDetailsFactory
    {
        /// <summary>
        /// The token service
        /// </summary>
        private readonly ITokenService _tokenService;

        private readonly ITPSupport _support;

        /// <summary>Initializes a new instance of the <see cref="TransferDetailsFactory" /> class.</summary>
        /// <param name="tokenService">The token service, that encodes and decodes response and request tokens.</param>
        public ExtraDetailsFactory(
            ITokenService tokenService,
            ITPSupport support)
        {
            _tokenService = Ensure.IsNotNull(tokenService, nameof(tokenService));
            _support = Ensure.IsNotNull(support, nameof(support));
        }
        public Task<ExtraDetails> CreateAsync(Book.Request request)
        {
            throw new NotImplementedException();
        }

        public async Task<ExtraDetails> CreateAsync(Prebook.Request request)
        {
            var extraDetails = new ExtraDetails();

            var extraToken = await _tokenService.DecodeExtraTokenAsync(request.BookingToken, request.Account);

            if (extraToken is not null)
            {
                extraDetails = new ExtraDetails()
                {
                    AccountID = request.Account.AccountID,
                    DepartureDate = extraToken.DepartureDate,
                    DepartureTime = extraToken.DepartureTime,
                    OneWay = extraToken.OneWay,
                    Source = extraToken.Source,
                    SupplierID = extraToken.SupplierID,
                    ISOCurrencyCode = await _support.ISOCurrencyCodeLookupAsync(extraToken.ISOCurrencyID),
                    ThirdPartyConfigurations = request.Account.Configurations,
                    Adults = extraToken.Adults,
                    Children = extraToken.Children,
                    Infants = extraToken.Infants,
                    //LocalCost = 123M, 
                    SupplierReference = request.SupplierReference,
                    ThirdPartySettings = request.ThirdPartySettings
                };

                if (!extraDetails.OneWay)
                {
                    extraDetails.ReturnDate = extraDetails.DepartureDate.AddDays(extraToken.Duration);
                    extraDetails.ReturnTime = extraToken.ReturnTime;
                }
            }

            Validate(extraToken!, extraDetails);

            return extraDetails;
        }

        #region Private functions

        /// <summary>Validates wether the extra details matches the expected values from the token</summary>
        /// <param name="extraToken">The extra token.</param>
        /// <param name="transferDetails">The extra details.</param>
        private void Validate(ExtraToken extraToken, ExtraDetails extraDetails)
        {
            if (extraToken == null
                || extraToken.DepartureDate == DateTime.MinValue)
            {
                extraDetails.Warnings.AddNew("Validate failure", WarningMessages.InvalidBookingToken);
            }
        }

        #endregion
    }
}
