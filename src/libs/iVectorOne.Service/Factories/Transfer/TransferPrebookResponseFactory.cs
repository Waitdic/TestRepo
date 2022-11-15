namespace iVectorOne.Factories
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Intuitive;
    using iVectorOne.Lookups;
    using iVectorOne.Models.Transfer;
    using iVectorOne.Models.Tokens;
    using iVectorOne.Repositories;
    using iVectorOne.SDK.V2.TransferPrebook;
    using iVectorOne.Services;
    using Search = SDK.V2.TransferSearch;
    using iVectorOne.Utility;
    using System;

    /// <summary>
    /// A factory that creates transfer pre book responses using the provided transfer details
    /// </summary>
    /// <seealso cref="ITransferPrebookResponseFactory" />
    public class TransferPrebookResponseFactory : ITransferPrebookResponseFactory
    {
        private readonly ITPSupport _support;

        /// <summary>The token service</summary>
        private readonly ITokenService _tokenService;

        /// <summary>Initializes a new instance of the <see cref="TransferPrebookResponseFactory" /> class.</summary>
        /// <param name="tokenService">The token service, that encodes and decodes response and request tokens</param>
        /// <param name="support">The third party support interface</param>
        public TransferPrebookResponseFactory(
            ITokenService tokenService,
            ITPSupport support)
        {
            _tokenService = Ensure.IsNotNull(tokenService, nameof(tokenService));
            _support = Ensure.IsNotNull(support, nameof(support));
        }

        /// <summary>
        /// Creates a pre book response using information from the transfer details
        /// </summary>
        /// <param name="transferDetails">The transfer details which contain all the information from the third party pre book</param>
        /// <returns>A pre book response</returns>
        public async Task<Response> CreateAsync(TransferDetails transferDetails)
        {
            var errata = new List<string>();
            //var cancellationTerms = new List<Search.CancellationTerm>();
           
            int isoCurrencyId = !string.IsNullOrEmpty(transferDetails.ISOCurrencyCode) ?
                await _support.ISOCurrencyIDLookupAsync(transferDetails.ISOCurrencyCode) :
                0;

            foreach (var cancellation in transferDetails.Cancellations)
            {
                //var cancellationTerm = new Search.CancellationTerm()
                //{
                //    Amount = cancellation.Amount + 0.00M,
                //    StartDate = cancellation.StartDate,
                //    EndDate = cancellation.EndDate,
                //};
                //cancellationTerms.Add(cancellationTerm);
            }


            var bookingToken = new TransferToken()
            {
                DepartureDate = transferDetails.DepartureDate,
            };

            var response = new Response()
            {
                BookingToken = _tokenService.EncodeTransferToken(bookingToken),
                TotalCost = transferDetails.LocalCost + 0.00M,
                //CancellationTerms = cancellationTerms,
                //SupplierReference = transferDetails.TPRef1,
            };

            return response;
        }
    }
}