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
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;
    using Constant = Models.Constant;

    public abstract class TourPlanTransfersSearchBase : IThirdPartySearch, ISingleSource
    {
        public abstract string Source { get; }

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

        public Task<List<Request>> BuildSearchRequestsAsync(TransferSearchDetails searchDetails, LocationMapping location)
        {
            LocationData tpLocations = GetThirdPartyLocations(location);

            XmlDocument departureBuildOptionInfoRequest = BuildOptionInfoRequest(searchDetails, tpLocations, searchDetails.DepartureDate);
            List<Request> request = new List<Request>();
            request.Add(GetXMLRequest(searchDetails));
            request[0].SetRequest(JsonConvert.SerializeObject(departureBuildOptionInfoRequest));
            if (!searchDetails.OneWay)
            {
                XmlDocument returnBuildOptionInfoRequest = BuildOptionInfoRequest(searchDetails, tpLocations, searchDetails.ReturnDate);
                request.Add(GetXMLRequest(searchDetails));
                request[1].SetRequest(JsonConvert.SerializeObject(returnBuildOptionInfoRequest));
            }

            return Task.FromResult( request);
        }

        private XmlDocument BuildOptionInfoRequest(TransferSearchDetails searchDetails, LocationData tpLocations, DateTime dateFrom)
        {
            OptionInfoRequest optionInfoRequest = new OptionInfoRequest()
            {
                AgentID = _settings.AgentId(searchDetails), 
                Password = _settings.Password(searchDetails), 
                DateFrom = dateFrom.ToString(Constant.DateTimeFormat) ,
                Info = "GSIT",
                Opt = tpLocations.LocationCode + "TR????????????",
                RoomConfigs = new List<RoomConfig>()
                {
                   new RoomConfig() {
                   Adults = searchDetails.Adults,
                   Children = searchDetails.Children,
                   Infants = searchDetails.Children
                   }
                }
            };

            XmlDocument xmlRequest = _serializer.SerializeWithoutNamespaces(optionInfoRequest);
            xmlRequest.InnerXml = $"<Request>{xmlRequest.InnerXml}</Request>";

            return xmlRequest;
        }

        private Request GetXMLRequest(TransferSearchDetails searchDetails)
        {
            return new Request()
            {
                EndPoint = _settings.URL(searchDetails),
                Method = RequestMethod.POST,
                ContentType = ContentTypes.Application_xml

            };
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
            return false;
        }

        public bool SearchRestrictions(TransferSearchDetails searchDetails)
        {
            return false;
            throw new System.NotImplementedException();
        }

        public TransformedTransferResultCollection TransformResponse(List<Request> requests, TransferSearchDetails searchDetails, LocationMapping location)
        {
            throw new System.NotImplementedException();
        }
    }
}
