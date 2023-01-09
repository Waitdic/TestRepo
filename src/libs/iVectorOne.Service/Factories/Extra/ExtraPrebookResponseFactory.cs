namespace iVectorOne.Factories
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Intuitive;
    using iVectorOne.Lookups;
    using iVectorOne.Models.Tokens.Transfer;
    using iVectorOne.SDK.V2.ExtraPrebook;
    using iVectorOne.Services;
    using iVectorOne.SDK.V2;
    using iVectorOne.Models.Extra;

    /// <summary>
    /// A factory that creates transfer pre book responses using the provided transfer details
    /// </summary>
    /// <seealso cref="ITransferPrebookResponseFactory" />
    public class ExtraPrebookResponseFactory : IExtraPrebookResponseFactory
    {
        private readonly ITPSupport _support;

        /// <summary>The token service</summary>
        private readonly ITokenService _tokenService;

        /// <summary>Initializes a new instance of the <see cref="TransferPrebookResponseFactory" /> class.</summary>
        /// <param name="tokenService">The token service, that encodes and decodes response and request tokens</param>
        /// <param name="support">The third party support interface</param>
        public ExtraPrebookResponseFactory(
            ITokenService tokenService,
            ITPSupport support)
        {
            _tokenService = Ensure.IsNotNull(tokenService, nameof(tokenService));
            _support = Ensure.IsNotNull(support, nameof(support));
        }

        /// <summary>
        /// Creates a pre book response using information from the transfer details
        /// </summary>
        /// <param name="transferDetails">The extra details which contain all the information from the third party pre book</param>
        /// <returns>A pre book response</returns>
        public async Task<Response> CreateAsync(ExtraDetails extraDetails)
        {
            var departureErrata = new List<string>();
            var returnErrata = new List<string>();
            var cancellationTerms = new List<CancellationTerm>();

            int isoCurrencyId = !string.IsNullOrEmpty(extraDetails.ISOCurrencyCode) ?
                await _support.ISOCurrencyIDLookupAsync(extraDetails.ISOCurrencyCode) : 0;

            foreach (var erratum in extraDetails.DepartureErrata)
            {
                departureErrata.Add(string.Join(": ", erratum.Title, erratum.Text));
            }

            foreach (var erratum in extraDetails.ReturnErrata)
            {
                returnErrata.Add(string.Join(": ", erratum.Title, erratum.Text));
            }

            foreach (var cancellation in extraDetails.Cancellations)
            {
                var cancellationTerm = new CancellationTerm()
                {
                    Amount = cancellation.Amount + 0.00M,
                    StartDate = cancellation.StartDate,
                    EndDate = cancellation.EndDate,
                };
                cancellationTerms.Add(cancellationTerm);
            }

            var transferToken = new TransferToken()
            {
                DepartureDate = extraDetails.DepartureDate,
                DepartureTime = extraDetails.DepartureTime,
                Duration = extraDetails.Duration,
                OneWay = extraDetails.OneWay,
                ReturnTime = extraDetails.ReturnTime,
                ISOCurrencyID = isoCurrencyId,
                Adults = extraDetails.Adults,
                Children = extraDetails.Children,
                Infants = extraDetails.Infants,
                SupplierID = await _support.SupplierIDLookupAsync(extraDetails.Source),
            };

            var response = new Response()
            {
                BookingToken = _tokenService.EncodeTransferToken(transferToken),
                SupplierReference = extraDetails.SupplierReference,
                TotalCost = extraDetails.LocalCost + 0.00M,
                CancellationTerms = cancellationTerms,
                DepartureErrata = departureErrata,
                ReturnErrata = returnErrata,
            };

            return response;
        }
    }
}