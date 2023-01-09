namespace iVectorOne.Factories
{
    using Intuitive;
    using iVectorOne.Factories;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Tokens.Transfer;
    using iVectorOne.SDK.V2.TransferSearch;
    using iVectorOne.Search.Models;
    using iVectorOne.Services;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    ///  Class responsible for building the transfer search response
    /// </summary>
    public class TransferSearchResponseFactory : ITransferSearchResponseFactory
    {
        /// <summary>The third party support service</summary>
        private readonly ITPSupport _support;

        /// <summary>The token service</summary>
        private ITokenService _tokenService;

        /// <summary>The log writer</summary>
        private readonly ILogger<TransferSearchResponseFactory> _logger;

        /// <summary>Initializes a new instance of the <see cref="TransferSearchResponseFactory" /> class.</summary>
        /// /// <param name="support">The third party support service</param>
        /// <param name="tokenService">
        ///   <para>The token service, that encodes and decodes response and request tokens</para>
        /// </param>
        /// <param name="logger">The log writer</param>
        public TransferSearchResponseFactory(
            ITPSupport support,
            ITokenService tokenService,
            ILogger<TransferSearchResponseFactory> logger)
        {
            _support = Ensure.IsNotNull(support, nameof(support));
            _tokenService = Ensure.IsNotNull(tokenService, nameof(tokenService));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        /// <summary>Creates the specified results.</summary>
        /// <param name="searchDetails">The search details, used to retrieve information about the search e.g. duration that is not on the results</param>
        /// <param name="locationMapping">The transfer location mapping containing the third party location data</param>
        /// <param name="requestTracker">The request tracker, allows for analysis of reponse times</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public async Task<Response> CreateAsync(TransferSearchDetails searchDetails, LocationMapping locationMapping, IRequestTracker requestTracker)
        {
            var response = new Response();
            var createResponseTimer = new ThirdPartyRequestTime("CreateResponse");
            createResponseTimer.StartTotalTimer();            
           
            foreach (var result in searchDetails.Results.GetValidResults)
            {
                try
                {
                    var transferToken = new TransferToken()
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

                    var transferResult = new TransferResult()
                    {
                        BookingToken = _tokenService.EncodeTransferToken(transferToken),
                        SupplierReference = result.SupplierReference,
                        TPSessionID = result.TPSessionID,
                        TransferVehicle = result.TransferVehicle,
                        ReturnTime = result.ReturnTime,
                        VehicleCost = result.VehicleCost,
                        AdultCost = result.AdultCost,
                        ChildCost = result.ChildCost,
                        CurrencyCode = await _support.ISOCurrencyCodeLookupAsync(result.CurrencyID),
                        VehicleQuantity = result.VehicleQuantity,
                        Cost = result.Cost,
                        BuyingChannelCost = result.BuyingChannelCost,
                        OutboundInformation = result.OutboundInformation,
                        ReturnInformation = result.ReturnInformation,
                        OutboundCost = result.OutboundCost,
                        ReturnCost = result.ReturnCost,
                        OutboundXML = result.OutboundXML,
                        ReturnXML = result.ReturnXML,
                        OutboundTransferMinutes = result.OutboundTransferMinutes,
                        ReturnTransferMinutes = result.ReturnTransferMinutes,
                        OnRequest = result.OnRequest,
                    };

                    response.TransferResults.Add(transferResult);
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