namespace iVectorOne.Suppliers.TourPlanTransfers
{
    using Intuitive;
    using Intuitive.Helpers.Net;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using iVectorOne.Transfer;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public abstract class TourPlanTransfersSearchBase : IThirdPartySearch, ISingleSource
    {
        public abstract string Source { get; }

        private ITourPlanTransfersSettings _settings;

        private readonly HttpClient _httpClient;

        public TourPlanTransfersSearchBase(
            ITourPlanTransfersSettings settings,
            HttpClient httpClient)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
        }

        public Task<List<Request>> BuildSearchRequestsAsync(TransferSearchDetails searchDetails, LocationMapping location)
        {
            LocationData tpLocations = GetThirdPartyLocations(location);

            return Task.FromResult(new List<Request>() { new Request() {
                EndPoint = "",
                ExtraInfo = tpLocations.Validation() ? tpLocations : new LocationData(),
                Method=RequestMethod.POST,
                ContentType = ContentTypes.Application_xml

            } });
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
