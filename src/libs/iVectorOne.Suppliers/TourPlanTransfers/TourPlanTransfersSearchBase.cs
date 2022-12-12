namespace iVectorOne.Suppliers.TourPlanTransfers
{
    using Intuitive;
    using Intuitive.Helpers.Net;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using iVectorOne.Suppliers.TourPlanTransfers.Models;
    using iVectorOne.Transfer;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;
    using Constant = Models.Constant;

    public abstract class TourPlanTransfersSearchBase : IThirdPartySearch, ISingleSource
    {
        #region Constructor
        private ITourPlanTransfersSettings _settings;

        private readonly HttpClient _httpClient;
        private readonly ISerializer _serializer;
        private readonly ILogger<TourPlanTransfersSearchBase> _logger;

        public TourPlanTransfersSearchBase(
            ITourPlanTransfersSettings settings,
            HttpClient httpClient,
            ISerializer serializer,
            ILogger<TourPlanTransfersSearchBase> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }
        #endregion

        #region Properties
        public abstract string Source { get; }
        #endregion

        #region Public Functions
        public Task<List<Request>> BuildSearchRequestsAsync(TransferSearchDetails searchDetails, LocationMapping location)
        {
            LocationData tpLocations = GetThirdPartyLocations(location);
            var Outbound = BuildOptionInfoRequest(searchDetails, tpLocations, searchDetails.DepartureDate);
            List<Request> requests = new List<Request>();
            Outbound.ExtraInfo = Constant.Outbound;
            requests.Add(Outbound);
            if (!searchDetails.OneWay)
            {
                var returnBuildOptionInfoRequest = BuildOptionInfoRequest(searchDetails, tpLocations, searchDetails.ReturnDate);
                returnBuildOptionInfoRequest.ExtraInfo = string.Empty;
                requests.Add(returnBuildOptionInfoRequest);
            }

            return Task.FromResult(requests);
        }
        public LocationData GetThirdPartyLocations(LocationMapping location)
        {
            LocationData locationData = new LocationData();
            if (location.DepartureData.Length > 0 && location.ArrivalData.Length > 0)
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
            foreach (Request request in requests)
            {
                if (!ResponseHasExceptions(request))
                {
                    deserializedResponse = DeSerialize<OptionInfoReply>(request.ResponseXML);
                    if ((string)request.ExtraInfo == Constant.Outbound)
                    {
                        filteredOutbound = FilterResults(tpLocations.DepartureName, tpLocations.ArrivalName, deserializedResponse);
                    }
                    if ((string)request.ExtraInfo != Constant.Outbound)
                    {
                        filteredReturn = FilterResults(tpLocations.ArrivalName, tpLocations.DepartureName, deserializedResponse);
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
                    supplierReference = CreateSupplierReference(outboundResult.Opt, outboundResult.OptStayResults.RateId, "", "");
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
                        supplierReference = CreateSupplierReference(outboundResult.Opt, outboundResult.OptStayResults.RateId, returnResult.Opt, returnResult.OptStayResults.RateId);
                        transformedResult = BuildTransformedResult(supplierReference, returnResult.OptGeneral.Comment, returnResult.OptStayResults.Currency, returnResult.OptStayResults.TotalPrice + outboundResult.OptStayResults.TotalPrice);
                        transformedResultList.Add(transformedResult);
                    }
                }
            }
            if (transformedResultList.Any())
            {
                TransformedTransferResultCollection.TransformedResults.AddRange(transformedResultList);
            }

            return TransformedTransferResultCollection;
        }

        private TransformedTransferResult BuildTransformedResult(string supplierReference, string comment, string currency, int totalPrice)
        {
            var transformedResult = new TransformedTransferResult()
            {
                TPSessionID = "",
                SupplierReference = supplierReference,
                TransferVehicle = comment,
                ReturnTime = "12:00",
                VehicleCost = 0,
                AdultCost = 0,
                ChildCost = 0,
                CurrencyCode = currency,
                VehicleQuantity = 1,
                Cost = totalPrice,
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
        private string CreateSupplierReference(string outboundOpt, string outboundRateId, string returnOpt, string returnRateId)
        {
            var reference = outboundOpt + "-" + outboundRateId;
            if (!string.IsNullOrEmpty(returnOpt))
            {
                reference += "|" + returnOpt + "-" + returnRateId;
            }
            return reference;
        }

        private OptionInfoReply FilterResults(string departureName, string arrivalName, OptionInfoReply deserializedResponse)
        {
            OptionInfoReply filterResult = new();
            var result = deserializedResponse.Option.ToList().Where(x => filterDescription(x.OptGeneral.Description, departureName, arrivalName)).ToList();
            if (result.Any())
            {
                filterResult.Option.AddRange(result);
            }
            return filterResult;
        }

        private bool filterDescription(string description, string departureName, string arrivalName)
        {
            bool result = false;
            try
            {
                List<string> splitDescriptionLocation = SplitDescription(description);
                result = splitDescriptionLocation.Count == 2 ? (splitDescriptionLocation[0] == departureName && splitDescriptionLocation[1] == arrivalName) : false;
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

                AgentID = _settings.AgentId(searchDetails),
                Password = _settings.Password(searchDetails),
                DateFrom = dateFrom.ToString(Constant.DateTimeFormat),
                Info = Constant.Info,
                Opt = tpLocations.LocationCode + Constant.TransferOptText,
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
            var xmlDocument = Serialize(optionInfoRequest);
            request.SetRequest(xmlDocument);

            return request;
        }
        private Request GetXMLRequest(TransferSearchDetails searchDetails)
        {
            return new Request()
            {
                EndPoint = _settings.URL(searchDetails),
                Method = RequestMethod.POST,
                ContentType = ContentTypes.Text_xml

            };
        }

        private XmlDocument Serialize(OptionInfoRequest request)
        {
            var xmlRequest = _serializer.SerializeWithoutNamespaces(request);
            xmlRequest.InnerXml = $"<Request>{xmlRequest.InnerXml}</Request>";
            return xmlRequest;
        }

        private T DeSerialize<T>(XmlDocument xmlDocument) where T : class
        {
            var xmlResponse = _serializer.CleanXmlNamespaces(xmlDocument);
            xmlResponse.InnerXml = xmlResponse.InnerXml.Replace("<Reply>", "").Replace("</Reply>", "");
            return _serializer.DeSerialize<T>(xmlResponse);
        }
        private List<string> SplitDescription(string description)
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
