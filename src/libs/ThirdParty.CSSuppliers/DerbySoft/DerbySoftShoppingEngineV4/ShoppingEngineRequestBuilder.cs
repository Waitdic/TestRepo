namespace ThirdParty.CSSuppliers.DerbySoft.DerbySoftShoppingEngineV4
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using iVector.Search.Property;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Net.WebRequests;
    using Newtonsoft.Json;
    using ThirdParty;
    using ThirdParty.CSSuppliers.DerbySoft.DerbySoftShoppingEngineV4.Models;
    using ThirdParty.CSSuppliers.DerbySoft.Models;
    using ThirdParty.Models;
    using ThirdParty.Search.Models;

    public class ShoppingEngineRequestBuilder : ISearchRequestBuilder
    {
        private readonly IDerbySoftSettings _settings;
        private readonly string _source;
        private readonly Guid _guid;

        public ShoppingEngineRequestBuilder(IDerbySoftSettings settings, string source, Guid guid)
        {
            _settings = settings;
            _source = source;
            _guid = guid;
        }

        public IEnumerable<Request> BuildSearchRequests(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var tpKeys = resortSplits
                .SelectMany(rs => rs.Hotels)
                .Select(h => h.TPKey);

            var uniqueCode = resortSplits.Any()
                ? $"{_source}_{resortSplits.First().ResortCode}"
                : _source;

            return searchDetails.RoomDetails
                .SelectMany((room, index) =>
                    BuildSearchRequests(
                        tpKeys,
                        searchDetails,
                        room,
                        uniqueCode,
                        index + 1));
        }

        public IEnumerable<Request> BuildSearchRequests(
            IEnumerable<string> tpKeys,
            SearchDetails searchDetails,
            RoomDetail roomDetails,
            string uniqueCode,
            int propertyRoomBookingId) =>
            tpKeys
                .Batch(_settings.ShoppingEngineHotelsBatchSize(searchDetails, _source))
                .Select(batch => BuildSearchRequest(batch, searchDetails, roomDetails))
                .Select(request => BuildWebRequest(
                    searchDetails,
                    request,
                    propertyRoomBookingId,
                    uniqueCode));

        private DerbySoftShoppingEngineV4SearchRequest BuildSearchRequest(
            IEnumerable<string> tpKeysBatch,
            SearchDetails searchDetails,
            RoomDetail roomDetails)
        {
            return new DerbySoftShoppingEngineV4SearchRequest
            {
                Header = BuildHeader(searchDetails),
                Hotels = tpKeysBatch.Select(tpKey => BuildHotel(searchDetails, tpKey)).ToArray(),
                StayRange = BuildStayRange(searchDetails),
                RoomCriteria = BuildRoomCriteria(roomDetails)
            };
        }

        private Header BuildHeader(IThirdPartyAttributeSearch searchDetails) =>
            new()
            {
                DistributorId = _settings.User(searchDetails, _source),
                Token = _guid.ToSafeString(),
                Version = "v4"
            };

        private static RoomCriteria BuildRoomCriteria(RoomDetail roomDetails) =>
            new()
            {
                RoomCount = 1,
                AdultCount = roomDetails.Adults,
                ChildCount = roomDetails.Children,
                ChildAges = roomDetails.ChildAges.ToArray()
            };

        private static StayRange BuildStayRange(ISearchDetails searchDetails)
        {
            return new StayRange
            {
                CheckIn = searchDetails.ArrivalDate.Date,
                CheckOut = searchDetails.DepartureDate.Date
            };
        }

        private CSSuppliers.DerbySoft.Models.Hotel BuildHotel(IThirdPartyAttributeSearch searchDetails, string tpKey) =>
            new()
            {
                HotelId = tpKey,
                SupplierId = _settings.SupplierID(searchDetails, _source),
                Status = "Actived"
            };

        private Request BuildWebRequest(
            SearchDetails searchDetails,
            DerbySoftShoppingEngineV4SearchRequest deserialisedRequest,
            int propertyRoomBookingId,
            string uniqueCode)
        {
            var request = new Request
            {
                EndPoint = _settings.ShoppingEngineURL(searchDetails, _source),
                Method = eRequestMethod.POST,
                ContentType = ContentTypes.Application_json,
                Accept = "application/json",
                ExtraInfo = propertyRoomBookingId,
            };

            var serialisedRequest = JsonConvert.SerializeObject(deserialisedRequest, DerbySoftSupport.GetJsonSerializerSettings());
            request.SetRequest(serialisedRequest);

            var password = _settings.ShoppingEnginePassword(searchDetails, _source);
            request.Headers.AddNew(
                "Authorization",
                "Bearer " + (!string.IsNullOrWhiteSpace(password) ? password : _settings.Password(searchDetails, _source)));

            return request;
        }
    }
}
