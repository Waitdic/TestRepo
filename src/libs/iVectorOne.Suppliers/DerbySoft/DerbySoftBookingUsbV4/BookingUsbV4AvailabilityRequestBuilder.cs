namespace iVectorOne.CSSuppliers.DerbySoft.DerbySoftBookingUsbV4
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using Newtonsoft.Json;
    using iVectorOne.CSSuppliers.DerbySoft.DerbySoftBookingUsbV4.Models;
    using iVectorOne.CSSuppliers.DerbySoft.Models;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;

    public class BookingUsbV4AvailabilityRequestBuilder : ISearchRequestBuilder
    {
        private readonly IDerbySoftSettings _settings;

        private readonly string _source;

        private readonly Guid _guid;

        public BookingUsbV4AvailabilityRequestBuilder(IDerbySoftSettings settings, string source, Guid guid)
        {
            _settings = settings;
            _source = source;
            _guid = guid;
        }

        public IEnumerable<Request> BuildSearchRequests(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = new List<Request>();
            int hotelSearchLimit = _settings.HotelBatchLimit(searchDetails, _source);

            var hotelKeys = resortSplits
                .SelectMany(rs => rs.Hotels)
                .Select(h => h.TPKey)
                .Take(hotelSearchLimit)
                .ToList();

            foreach (string hotelKey in hotelKeys)
            {
                int propertyRoomBookingID = 1;

                foreach (var roomDetail in searchDetails.RoomDetails)
                {
                    string uniqueCode = _source;

                    if (resortSplits.Count > 1)
                    {
                        uniqueCode = $"{_source}_{resortSplits.First().ResortCode}";
                    }

                    var availabilityRequest = GetAvailabilityRequest(searchDetails, roomDetail, hotelKey);

                    var request = new Request
                    {
                        EndPoint = _settings.SearchURL(searchDetails, _source),
                        Method = RequestMethod.POST,
                        Source = _source,
                        ContentType = ContentTypes.Application_json,
                        Accept = "application/json",
                        UseGZip = _settings.UseGZip(searchDetails, _source),
                        ExtraInfo = propertyRoomBookingID,
                        TimeoutInSeconds = 100,
                    };

                    string availabilityRequestString = JsonConvert.SerializeObject(availabilityRequest, DerbySoftSupport.GetJsonSerializerSettings());
                    request.SetRequest(availabilityRequestString);

                    request.Headers.AddNew("Authorization", "Bearer " + _settings.Password(searchDetails, _source));
                    requests.Add(request);

                    propertyRoomBookingID++;
                }
            }

            return requests;
        }

        private DerbySoftBookingUsbV4AvailabilityRequest GetAvailabilityRequest(
            SearchDetails searchDetails,
            iVector.Search.Property.RoomDetail roomDetail,
            string tpKey)
        {
            var header = new Header
            {
                SupplierId = _settings.SupplierID(searchDetails, _source),
                DistributorId = _settings.User(searchDetails, _source),
                Token = _guid.ToSafeString(),
                Version = "v4"
            };

            var stayRange = new StayRange
            {
                CheckIn = searchDetails.ArrivalDate,
                CheckOut = searchDetails.ArrivalDate.AddDays(searchDetails.Duration)
            };

            var roomCriteria = new RoomCriteria
            {
                RoomCount = 1,
                AdultCount = roomDetail.Adults,
                ChildCount = roomDetail.Children + roomDetail.Infants,
                ChildAges = roomDetail.ChildAndInfantAges(1).ToArray()
            };

            var availabilityRequest = new DerbySoftBookingUsbV4AvailabilityRequest
            {
                Header = header,
                HotelId = tpKey,
                StayRange = stayRange,
                RoomCriteria = roomCriteria
            };

            return availabilityRequest;
        }
    }
}