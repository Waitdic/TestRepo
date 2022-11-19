namespace iVectorOne.Factories
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Intuitive;
    using iVectorOne.Lookups;
    using iVectorOne.Models.Transfer;
    using iVectorOne.Models.Tokens.Transfer;
    using iVectorOne.Repositories;
    using iVectorOne.SDK.V2.TransferPrebook;
    using iVectorOne.Services;
    using Search = SDK.V2.TransferSearch;
    using iVectorOne.Utility;
    using System;
    using iVectorOne.SDK.V2;
    using iVectorOne.Search.Models;
    using iVector.Search.Property;

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
            var cancellationTerms = new List<CancellationTerm>();

            int isoCurrencyId = !string.IsNullOrEmpty(transferDetails.ISOCurrencyCode) ?
                await _support.ISOCurrencyIDLookupAsync(transferDetails.ISOCurrencyCode) : 0;

            foreach (var cancellation in transferDetails.Cancellations)
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
                DepartureDate = transferDetails.DepartureDate,
                DepartureTime = transferDetails.DepartureTime,
                Duration = transferDetails.Duration,
                OneWay = transferDetails.OneWay,
                ReturnTime = transferDetails.ReturnTime,
                ISOCurrencyID = isoCurrencyId,
                Adults = transferDetails.Adults,
                Children = transferDetails.Children,
                Infants = transferDetails.Infants,
                SupplierID = await _support.SupplierIDLookupAsync(transferDetails.Source),
            };

            var response = new Response()
            {
                BookingToken = _tokenService.EncodeTransferToken(transferToken),
                SupplierReference = transferDetails.SupplierReference,
                TotalCost = transferDetails.LocalCost + 0.00M,
                CancellationTerms = cancellationTerms,
                DepartureNotes = transferDetails.DepartureNotes,
                ReturnNotes = transferDetails.ReturnNotes,
            };

            return response;
        }
    }
}