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

        public TourPlanTransfersSearchBase(
            ITourPlanTransfersSettings settings,
            HttpClient httpClient,
            ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }
        #endregion

        #region Properties
        public abstract string Source { get; }
        #endregion

        #region Public Functions
        public Task<List<Request>> BuildSearchRequestsAsync(TransferSearchDetails searchDetails, LocationMapping location)
        {
            LocationData tpLocations = GetThirdPartyLocations(location);
            var departureBuildOptionInfoRequest = BuildOptionInfoRequest(searchDetails, tpLocations, searchDetails.DepartureDate);
            List<Request> requests = new List<Request>();
            departureBuildOptionInfoRequest.ExtraInfo = Constant.OneWay;
            requests.Add(departureBuildOptionInfoRequest);
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
            var TransformedTransferResultCollection = new TransformedTransferResultCollection();
            LocationData tpLocations = GetThirdPartyLocations(location);
            foreach (Request request in requests)
            {
                if (!ResponseHasExceptions(request))
                {
                    OptionInfoReply deserializedResponse = DeSerialize<OptionInfoReply>(request.ResponseXML);
                    OptionInfoReply filterResult = new OptionInfoReply();
                    OptionInfoReply departureFilterResult = FilterResult(tpLocations.DepartureName, tpLocations.ArrivalName, deserializedResponse);
                    filterResult.Option.AddRange(departureFilterResult.Option);
                    string supplierReference = GetOneWaySupplierReference(departureFilterResult);
                    if (string.IsNullOrEmpty((string)request.ExtraInfo))
                    {
                        OptionInfoReply arivalFilterResult = FilterResult(tpLocations.ArrivalName, tpLocations.DepartureName, deserializedResponse);
                        filterResult.Option.AddRange(arivalFilterResult.Option);
                        supplierReference = GetTwoWaySupplierReference(departureFilterResult, arivalFilterResult);


                    }


                    foreach (var option in filterResult.Option)
                    {
                        var transformedResult = new TransformedTransferResult()
                        {
                            TPSessionID = "",
                            SupplierReference = supplierReference,
                            TransferVehicle = option.OptGeneral.Comment,
                            ReturnTime = "12:00",
                            VehicleCost = 0,
                            AdultCost = 0,
                            ChildCost = 0,
                            CurrencyCode = option.OptStayResults.Currency,
                            VehicleQuantity = 1,
                            Cost = option.OptStayResults.TotalPrice,
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
                        TransformedTransferResultCollection.TransformedResults.Add(transformedResult);
                    }
                }
            }

            return TransformedTransferResultCollection;
        }


        #endregion

        #region Private Functions
        private string GetOneWaySupplierReference(OptionInfoReply departureResult)
        {
            return departureResult.Option.Any() ? departureResult.Option.FirstOrDefault().Opt + "-" +
                    departureResult.Option.FirstOrDefault().OptStayResults.RateId : string.Empty;
        }
        private string GetTwoWaySupplierReference(OptionInfoReply departureResult, OptionInfoReply arivalResult)
        {
            string result = string.Empty;
            if (departureResult.Option.Any() && arivalResult.Option.Any())
            {
                result = departureResult.Option.FirstOrDefault().Opt + "-" + departureResult.Option.FirstOrDefault().OptStayResults.RateId + "|" +
                    arivalResult.Option.FirstOrDefault().Opt + "-" + arivalResult.Option.FirstOrDefault().OptStayResults.RateId;
            }

            return result;
        }

        private OptionInfoReply FilterResult(string departureName, string arrivalName, OptionInfoReply deserializedResponse)
        {
            OptionInfoReply filterResult = new OptionInfoReply();
            var result = deserializedResponse.Option.ToList().Where(x => filterDescription(x.OptGeneral.Description, departureName, arrivalName) && x.OptStayResults.Availability != "OK").ToList();
            filterResult.Option.AddRange(result);
            return filterResult;
        }

        private bool filterDescription(string description, string departureName, string arrivalName)
        {
            List<string> splitDescriptionLocation = SplitDescription(description);
            return (splitDescriptionLocation[0] == departureName && splitDescriptionLocation[1] == arrivalName);
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
