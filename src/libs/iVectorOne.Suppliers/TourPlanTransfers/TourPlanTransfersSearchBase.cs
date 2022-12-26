namespace iVectorOne.Suppliers.TourPlanTransfers
{
    using Intuitive;
    using Intuitive.Helpers.Net;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using iVectorOne.Services.Transfer;
    using iVectorOne.Suppliers.TourPlanTransfers.Models;
    using iVectorOne.Transfer;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;
    using Constants = Models.Constant;

    public abstract class TourPlanTransfersSearchBase : IThirdPartySearch, ISingleSource
    {
        #region Constructor
        private readonly ITourPlanTransfersSettings _settings;

        private readonly HttpClient _httpClient;
        private readonly ISerializer _serializer;
        private readonly ILocationManagerService _locationManagerService;
        private readonly ILogger<TourPlanTransfersSearchBase> _logger;
        public static readonly string ThirdPartySettingException = "The Third Party Setting: {0} must be provided.";

        public TourPlanTransfersSearchBase(
            HttpClient httpClient,
            ISerializer serializer,
            ILogger<TourPlanTransfersSearchBase> logger,
            ILocationManagerService locationManagerService
           )
        {
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
            _locationManagerService = Ensure.IsNotNull(locationManagerService, nameof(locationManagerService));
            _settings = new InjectedTourPlanTransfersSettings();
        }
        #endregion

        #region Properties
        public abstract string Source { get; }
        public virtual string TransferOptText { get; } = Constants.TransferOptText;
        #endregion

        #region Public Functions
        public bool ValidateSettings(TransferSearchDetails searchDetails)
        {
            if (!_settings.SetThirdPartySettings(searchDetails.ThirdPartySettings))
            {
                searchDetails.Warnings.AddRange(_settings.GetWarnings());
                return false;
            }
            return true;
        }

        public Task<List<Request>> BuildSearchRequestsAsync(TransferSearchDetails searchDetails, LocationMapping location)
        {
            List<Request> requests = new List<Request>();
            LocationData tpLocations = GetThirdPartyLocations(location);
            if (!string.IsNullOrEmpty(tpLocations.LocationCode))
            {
                var Outbound = BuildOptionInfoRequest(searchDetails, tpLocations, searchDetails.DepartureDate);
                Outbound.ExtraInfo = Constants.Outbound;
                requests.Add(Outbound);
                if (!searchDetails.OneWay)
                {
                    var returnBuildOptionInfoRequest = BuildOptionInfoRequest(searchDetails, tpLocations, searchDetails.ReturnDate);
                    returnBuildOptionInfoRequest.ExtraInfo = string.Empty;
                    requests.Add(returnBuildOptionInfoRequest);
                }
            }
            return Task.FromResult(requests);

        }
        public LocationData GetThirdPartyLocations(LocationMapping location)
        {
            LocationData locationData = new LocationData();
            if (location != null && (location.DepartureData.Length > 0 && location.ArrivalData.Length > 0))
            {
                string[] departureData = location.DepartureData.Split(":");
                string[] arrivalData = location.ArrivalData.Split(":");
                if (locationData.IsLocationDataValid(arrivalData) &&
                    locationData.IsLocationDataValid(departureData))
                {
                    locationData.ArrivalName = arrivalData[1].TrimStart();
                    locationData.DepartureName = departureData[1].TrimStart();
                    if (arrivalData[0].Equals(departureData[0]))
                    {
                        locationData.LocationCode = arrivalData[0];
                    }
                }
            }

            return locationData;

        }

        public bool ResponseHasExceptions(Request request)
        {
            return request.ResponseXML.OuterXml.Contains("<ErrorReply>");
        }

        public bool SearchRestrictions(TransferSearchDetails searchDetails)
        {
            return false;
            throw new System.NotImplementedException();
        }

        public TransformedTransferResultCollection TransformResponse(List<Request> requests, TransferSearchDetails searchDetails, LocationMapping location)
        {
            TransformedTransferResultCollection TransformedTransferResultCollection = new TransformedTransferResultCollection();
            LocationData tpLocations = GetThirdPartyLocations(location);
            bool oneway = requests.Count == 1;
            OptionInfoReply filteredOutbound = new();
            OptionInfoReply filteredReturn = new();
            OptionInfoReply deserializedResponse = new();
            List<string> uniqueLocationList = new();
            foreach (Request request in requests)
            {
                if (!ResponseHasExceptions(request))
                {
                    deserializedResponse = Helpers.DeSerialize<OptionInfoReply>(request.ResponseXML, _serializer);

                    if ((string)request.ExtraInfo == Constants.Outbound)
                    {
                        filteredOutbound = FilterResults(tpLocations.DepartureName, tpLocations.ArrivalName, deserializedResponse, ref uniqueLocationList);
                    }
                    if ((string)request.ExtraInfo != Constants.Outbound)
                    {
                        filteredReturn = FilterResults(tpLocations.ArrivalName, tpLocations.DepartureName, deserializedResponse, ref uniqueLocationList);
                    }
                }
            }
            string supplierReference;

            TransformedTransferResult? transformedResult = null;
            List<TransformedTransferResult> transformedResultList = new();
            if (oneway)
            {
                foreach (var outboundResult in filteredOutbound.Option)
                {
                    supplierReference = Helpers.CreateSupplierReference(outboundResult.Opt, outboundResult.OptStayResults.RateId, "", "");
                    transformedResult = BuildTransformedResult(supplierReference, outboundResult.OptGeneral.Comment, outboundResult.OptStayResults.Currency, outboundResult.OptStayResults.TotalPrice);
                    transformedResultList.Add(transformedResult);
                }
            }
            else
            {
                foreach (var outboundResult in filteredOutbound.Option)
                {
                    foreach (var returnResult in filteredReturn.Option.Where(x => x.OptGeneral.Comment == outboundResult.OptGeneral.Comment))
                    {
                        supplierReference = Helpers.CreateSupplierReference(outboundResult.Opt, outboundResult.OptStayResults.RateId, returnResult.Opt, returnResult.OptStayResults.RateId);
                        transformedResult = BuildTransformedResult(supplierReference, returnResult.OptGeneral.Comment, returnResult.OptStayResults.Currency, returnResult.OptStayResults.TotalPrice + outboundResult.OptStayResults.TotalPrice);
                        transformedResultList.Add(transformedResult);
                    }
                }
            }
            if (transformedResultList.Any())
            {
                TransformedTransferResultCollection.TransformedResults.AddRange(transformedResultList);
            }
            if (uniqueLocationList.Any())
            {
                _locationManagerService.CheckLocations(uniqueLocationList, searchDetails, tpLocations.LocationCode);
            }

            return TransformedTransferResultCollection;

        }

        private TransformedTransferResult BuildTransformedResult(string supplierReference, string comment, string currency, int totalPrice)
        {
            var transformedResult = new TransformedTransferResult()
            {
                TPSessionID = "",
                SupplierReference = supplierReference,
                TransferVehicle = comment.Substring(0, comment.Length <= 50 ? comment.Length : 50),
                ReturnTime = "12:00",
                VehicleCost = 0,
                AdultCost = 0,
                ChildCost = 0,
                CurrencyCode = currency,
                VehicleQuantity = 1,
                Cost = totalPrice / 100m,
                BuyingChannelCost = 0,
                OutboundInformation = "",
                ReturnInformation = "",
                OutboundCost = 0,
                ReturnCost = 0,
                OutboundXML = "",
                ReturnXML = "",
                OutboundTransferMinutes = 0,
                ReturnTransferMinutes = 0,
            };

            return transformedResult;
        }

        #endregion

        #region Private Functions
        private OptionInfoReply FilterResults(string departureName, string arrivalName, OptionInfoReply deserializedResponse, ref List<string> uniqueLocationList)
        {
            OptionInfoReply filterResult = new();
            List<string> filterUniqueLocation = new();

            List<Option> result = new();
            foreach (var option in deserializedResponse.Option.ToList().Where(x => x.OptStayResults.Availability == "OK"))
            {
                if (filterDescription(option.OptGeneral.Description, departureName, arrivalName, ref filterUniqueLocation))
                {
                    result.Add(option);
                }
                else
                {
                    filterUniqueLocation = filterUniqueLocation.Where(x => x != arrivalName && x != departureName).Except(uniqueLocationList).ToList();
                }

                if (filterUniqueLocation.Any())
                {
                    uniqueLocationList.AddRange(filterUniqueLocation);
                }
            }

            if (result.Any())
            {
                filterResult.Option.AddRange(result);
            }
            return filterResult;
        }

        private bool filterDescription(string description, string departureName, string arrivalName, ref List<string> filterUniqueLocation)
        {
            bool result = false;
            try
            {
                List<string> splitDescriptionLocation = SplitDescription(description);
                result = splitDescriptionLocation.Count == 2 ? (splitDescriptionLocation[0] == departureName && splitDescriptionLocation[1] == arrivalName) : false;
                if (!result && splitDescriptionLocation.Count == 2)
                {
                    filterUniqueLocation = new() { splitDescriptionLocation[0], splitDescriptionLocation[1] };
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, Constant.UnexpectedError);
            }

            return result;
        }

        private Request BuildOptionInfoRequest(TransferSearchDetails searchDetails, LocationData tpLocations, DateTime dateFrom)
        {
            Request request = new Request();
            OptionInfoRequest optionInfoRequest = new OptionInfoRequest()
            {

                AgentID = _settings.AgentId,
                Password = _settings.Password,
                DateFrom = dateFrom.ToString(Constants.DateTimeFormat),
                Info = Constants.Info,
                Opt = tpLocations.LocationCode + TransferOptText,
                RoomConfigs = new List<RoomConfiguration>()
                {
                   new RoomConfiguration() {
                   Adults = searchDetails.Adults,
                   Children = searchDetails.Children,
                   Infants = searchDetails.Children
                   }
                }
            };

            request = GetXMLRequest(searchDetails);
            var xmlDocument = Helpers.Serialize(optionInfoRequest, _serializer);
            request.SetRequest(xmlDocument);

            return request;
        }
        private Request GetXMLRequest(TransferSearchDetails searchDetails)
        {
            return new Request()
            {
                EndPoint = _settings.URL,
                Method = RequestMethod.POST,
                ContentType = ContentTypes.Text_xml

            };
        }
        public virtual List<string> SplitDescription(string description)
        {
            var list = new List<string>();

            try
            {
                description = description.Replace(",", "");

                var strings = description.Split(" to ");

                if (strings.Length == 2)
                {
                    list.Add(strings[0]);
                    list.Add(strings[1].Replace(" Transfer", ""));
                }
            }
            catch
            {
            }

            return list;
        }
        #endregion

    }
}