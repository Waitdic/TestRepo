namespace iVectorOne.Factories
{
    using Intuitive;
    using iVectorOne.Lookups;
    using iVectorOne.Models.Extra;
    using iVectorOne.Models.Tokens.Extra;
    using iVectorOne.SDK.V2.ExtraSearch;
    using iVectorOne.Search.Models;
    using iVectorOne.Services;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    ///  Class responsible for building the extra search response
    /// </summary>
    public class ExtraSearchResponseFactory : IExtraSearchResponseFactory
    {
        /// <summary>The third party support service</summary>
        private readonly ITPSupport _support;

        /// <summary>The token service</summary>
        private ITokenService _tokenService;

        /// <summary>The log writer</summary>
        private readonly ILogger<ExtraSearchResponseFactory> _logger;

        /// <summary>Initializes a new instance of the <see cref="ExtraSearchResponseFactory" /> class.</summary>
        /// /// <param name="support">The third party support service</param>
        /// <param name="tokenService">
        ///   <para>The token service, that encodes and decodes response and request tokens</para>
        /// </param>
        /// <param name="logger">The log writer</param>
        public ExtraSearchResponseFactory(
            ITPSupport support,
            ITokenService tokenService,
            ILogger<ExtraSearchResponseFactory> logger)
        {
            _support = Ensure.IsNotNull(support, nameof(support));
            _tokenService = Ensure.IsNotNull(tokenService, nameof(tokenService));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        /// <summary>Creates the specified results.</summary>
        /// <param name="searchDetails">The search details, used to retrieve information about the search e.g. duration that is not on the results</param>
        /// <param name="extras">The extras containing the third party extra names</param>
        /// <param name="requestTracker">The request tracker, allows for analysis of reponse times</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public async Task<Response> CreateAsync(ExtraSearchDetails searchDetails, List<Extra> extras, IRequestTracker requestTracker)
        {
            var response = new Response();
            var createResponseTimer = new ThirdPartyRequestTime("CreateResponse");
            createResponseTimer.StartTotalTimer();            
           
            foreach (var result in searchDetails.Results.GetValidResults)
            {
                try
                {
                    var extraToken = new ExtraToken()
                    {
                        DepartureDate = searchDetails.DepartureDate,
                        DepartureTime = searchDetails.DepartureTime,
                        Duration = searchDetails.Duration,
                        OneWay = searchDetails.OneWay,
                        ReturnTime = searchDetails.ReturnTime,
                        ISOCurrencyID = result.CurrencyID,
                        Adults = searchDetails.Adults,
                        Children = searchDetails.Children,
                        Infants = searchDetails.Infants,
                        SupplierID = await _support.SupplierIDLookupAsync(searchDetails.Source),
                    };

                    var extraResult = new ExtraResult()
                    {
                        BookingToken = _tokenService.EncodeExtraToken(extraToken),
                        SupplierReference = result.SupplierReference,
                        TPSessionID = result.TPSessionID,
                        UseDate= result.UseDate,
                        UseTime= result.UseTime,
                        EndDate= result.EndDate,
                        EndTime= result.EndTime,
                        ExtraName= result.ExtraName,
                        ExtraCategory= result.ExtraCategory,
                        AdditionalDetails= result.AdditionalDetails,
                        CurrencyCode = await _support.ISOCurrencyCodeLookupAsync(result.CurrencyID),
                        Cost = result.Cost,
                    };

                    response.ExtraResults.Add(extraResult);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "BuildResultsError");
                }
            }

            createResponseTimer.StopTotalTimer();
            createResponseTimer.SetTimes();
            requestTracker.RequestTimes.Add(createResponseTimer);

            return response;
        }
    }
}