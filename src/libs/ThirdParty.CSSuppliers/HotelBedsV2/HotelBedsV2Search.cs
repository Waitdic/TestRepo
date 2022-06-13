namespace ThirdParty.CSSuppliers.HotelBedsV2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Security;
    using Intuitive.Net.WebRequests;
    using iVector.Search.Property;
    using Newtonsoft.Json;
    using ThirdParty.Search.Models;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Interfaces;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.Results;
    using ThirdParty.Search.Support;

    public class HotelBedsV2Search : IThirdPartySearch, ISingleSource
    {
        #region Properties

        private readonly IHotelBedsV2Settings _settings;
        private readonly ITPSupport _support;
        private readonly ISecretKeeper _secretKeeper;

        public string Source => ThirdParties.HOTELBEDSV2;

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source) => false;

        #endregion

        #region Constructors

        public HotelBedsV2Search(
            IHotelBedsV2Settings settings,
            ITPSupport support,
            ISecretKeeper secretKeeper)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _secretKeeper = Ensure.IsNotNull(secretKeeper, nameof(secretKeeper));
        }

        #endregion

        public List<Request> BuildSearchRequests(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = new List<Request>();
            var hotelIDList = new List<int>();
            int batchLimit = _settings.BatchLimit(searchDetails);

            foreach (var searchResortSplit in resortSplits)
            {
                foreach (var oHotel in searchResortSplit.Hotels)
                {
                    if (hotelIDList.Count < batchLimit)
                    {
                        hotelIDList.Add(oHotel.TPKey.ToSafeInt());
                    }
                }
            }
            int limitPerBatch = _settings.BatchLimit(searchDetails);
            int totalHotels = hotelIDList.Count;
            int batchNumber = 1;
            var batches = new Dictionary<int, List<int>>();

            if (totalHotels > limitPerBatch)
            {
                var listofIDs = new List<int>();
                foreach (int hotelID in hotelIDList)
                {
                    if (listofIDs.Count == limitPerBatch)
                    {
                        batches.Add(batchNumber, listofIDs);
                        listofIDs = new List<int>();
                        batchNumber++;
                    }

                    listofIDs.Add(hotelID);
                }
                if (listofIDs.Any())
                {
                    batches.Add(batchNumber, listofIDs);
                }
            }
            else
            {
                batches.Add(batchNumber, hotelIDList);
            }

            foreach (KeyValuePair<int, List<int>> batch in batches)
            {
                HotelBedsV2AvailabilityRequest availabilityRequest = GetAvailabilityRequest(searchDetails, batch.Value);

                var searchHelper = new SearchHelper();
                searchHelper.SearchDetails = searchDetails;
                searchHelper.ResortSplit = resortSplits.FirstOrDefault();
                string sUniqueCode = Source;
                if (resortSplits.Count > 1)
                {
                    sUniqueCode = $"{Source}_{resortSplits.FirstOrDefault().ResortCode}";
                }
                searchHelper.UniqueRequestID = sUniqueCode;

                var request = new Request()
                {
                    EndPoint = _settings.URL(searchDetails),
                    Method = eRequestMethod.POST,
                    Source = ThirdParties.HOTELBEDSV2,
                    ContentType = ContentTypes.Application_json,
                    Accept = "application/json",
                    ExtraInfo = searchHelper,
                    TimeoutInSeconds = 100,
                };

                string availabilityRequestString = JsonConvert.SerializeObject(availabilityRequest);
                availabilityRequestString = RemoveBadRequestElements(availabilityRequestString);
                request.SetRequest(availabilityRequestString);

                request.Headers.AddNew("Api-key", _settings.User(searchDetails));
                request.Headers.AddNew("X-Signature", GetSignature(_settings.User(searchDetails), _settings.Password(searchDetails)));
                requests.Add(request);
            }

            return requests;
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();
            var allResponses = new List<HotelBedsV2AvailabilityResponse>();

            foreach (var request in requests)
            {
                var response = new HotelBedsV2AvailabilityResponse();
                bool success = request.Success;

                if (success)
                {
                    response = JsonConvert.DeserializeObject<HotelBedsV2AvailabilityResponse>(request.ResponseString);
                    allResponses.Add(response);
                }
            }

            transformedResults.TransformedResults
                .AddRange(allResponses.Where(o => o.hotels.total > 0)
                .SelectMany(r => GetResultFromResponse(searchDetails, r)));

            return transformedResults;
        }

        private List<TransformedResult> GetResultFromResponse(SearchDetails searchDetails, HotelBedsV2AvailabilityResponse response)
        {
            List<TransformedResult> transformedResults = new List<TransformedResult>();
            bool multiRoom = searchDetails.Rooms > 1;

            Dictionary<int, string> occupancyDictionary = new Dictionary<int, string>();
            int i = 1;
            foreach (var RoomDetail in searchDetails.RoomDetails)
            {
                string occupancyString = $"Adults:{RoomDetail.Adults},Children:{RoomDetail.Children + RoomDetail.Infants}";
                occupancyDictionary.Add(i, occupancyString);
                i++;
            }

            foreach (HotelBedsV2AvailabilityResponse.Hotel hotel in response.hotels.hotels)
            {
                foreach (HotelBedsV2AvailabilityResponse.Room room in hotel.rooms)
                {
                    var roomDictionary = new Dictionary<string, List<int>>();
                    foreach (HotelBedsV2AvailabilityResponse.Rate rate in room.rates)
                    {

                        string occupancyString = $"Adults:{rate.adults},Children:{rate.children}";
                        var occupancies = occupancyDictionary.Where(o => o.Value == occupancyString);
                        int numberOfOccupancies = occupancies.Count();
                        int propertyRoomBookingID = 0;
                        if (numberOfOccupancies == 1)
                        {
                            propertyRoomBookingID = occupancyDictionary.First(o => o.Value == occupancyString).Key;
                        }

                        decimal amount = rate.net.ToSafeDecimal();

                        if (!(_settings.ExcludeNRF(searchDetails) && rate.rateClass == "NRF"))
                        {
                            var cancellations = new Cancellations();
                            if (rate.cancellationPolicies != null)
                            {
                                var orderedCancellations = rate.cancellationPolicies
                                    .OrderBy(c => c.from)
                                    .ToList();

                                for (int iCanx = 0; iCanx < orderedCancellations.Count; iCanx++)
                                {
                                    var cancellation = orderedCancellations[iCanx];
                                    var endDate = searchDetails.ArrivalDate;
                                    if (iCanx < orderedCancellations.Count - 1)
                                    {
                                        endDate = orderedCancellations[iCanx + 1].from.AddDays(-1);
                                    }

                                    cancellations.Add(new ThirdParty.Models.Property.Booking.Cancellation
                                    {
                                        Amount = 100 * cancellation.amount.ToSafeDecimal() / amount,
                                        StartDate = cancellation.from,
                                        EndDate = endDate
                                    });
                                }
                            }

                            TransformedResult transformedResult = new TransformedResult()
                            {
                                RoomTypeCode = room.code,
                                MealBasisCode = rate.boardCode,
                                RoomType = room.name,
                                Amount = amount,
                                SellingPrice = amount.ToSafeString(),
                                NetPrice = amount.ToSafeString(),
                                MinimumPrice = rate.hotelMandatory ? rate.sellingRate.ToSafeDecimal() : 0,
                                CurrencyCode = hotel.currency,
                                TPReference = HotelBedsV2Reference.Create(rate.rateKey, rate.paymentType, rate.boardCode, _secretKeeper),
                                TPKey = hotel.code.ToSafeString(),
                                PropertyRoomBookingID = propertyRoomBookingID,
                                NonRefundableRates = rate.rateClass == "NRF",
                                Cancellations = cancellations
                            };

                            var key = $"{room.code}-{rate.boardCode}-{occupancyString}-{transformedResult.NonRefundableRates}";

                            if (transformedResult.PropertyRoomBookingID == 0)
                            {
                                int result = 0;
                                if (roomDictionary.TryGetValue(key, out var propertyRoomBookingIDs))
                                {
                                    var nextId = occupancies.FirstOrDefault(o => !propertyRoomBookingIDs.Contains(o.Key));
                                    result = nextId.Key;
                                    propertyRoomBookingIDs.Add(result);
                                }
                                else
                                {
                                    if (occupancies.Any())
                                    {
                                        result = occupancies.First().Key;
                                        roomDictionary.Add(key, new List<int>() { result });
                                    }
                                }

                                if (result == 0)
                                {
                                    result = occupancyDictionary.Count;
                                }
                                transformedResult.PropertyRoomBookingID = result;
                            }

                            transformedResults.Add(transformedResult);

                        }

                    }

                }
            }

            return transformedResults;
        }

        private HotelBedsV2AvailabilityRequest GetAvailabilityRequest(SearchDetails searchDetails, List<int> hotelIDList)
        {
            HotelBedsV2AvailabilityRequest hotelBedsV2AvailabilityRequest = new HotelBedsV2AvailabilityRequest();
            HotelBedsV2AvailabilityRequest.Stay stay = new HotelBedsV2AvailabilityRequest.Stay();
            var occupancies = new List<HotelBedsV2AvailabilityRequest.Occupancy>();

            string countryCode = "";

            if (searchDetails.LeadGuestBookingCountryID > 0)
            {
                countryCode = _support.TPBookingCountryLookup(Source, searchDetails.LeadGuestBookingCountryID);
            }

            if (string.IsNullOrWhiteSpace(countryCode))
            {
                countryCode = _settings.CustomerCountryCode(searchDetails);
            }

            hotelBedsV2AvailabilityRequest.sourceMarket = countryCode;

            stay.checkIn = searchDetails.ArrivalDate.ToString("yyyy-MM-dd");
            stay.checkOut = searchDetails.DepartureDate.ToString("yyyy-MM-dd");

            foreach (var roomDetail in searchDetails.RoomDetails)
            {
                HotelBedsV2AvailabilityRequest.Occupancy occupancy = new HotelBedsV2AvailabilityRequest.Occupancy();
                List<HotelBedsV2AvailabilityRequest.Pax> paxList = new List<HotelBedsV2AvailabilityRequest.Pax>();
                occupancy.adults = roomDetail.Adults;
                occupancy.rooms = 1;
                occupancy.children = roomDetail.Children + roomDetail.Infants;

                foreach (int childAge in roomDetail.ChildAges)
                {
                    HotelBedsV2AvailabilityRequest.Pax pax = new HotelBedsV2AvailabilityRequest.Pax();
                    pax.age = childAge;
                    pax.type = "CH";
                    paxList.Add(pax);
                }
                for (int adultIncrementer = 1; adultIncrementer <= roomDetail.Adults; adultIncrementer++)
                {
                    HotelBedsV2AvailabilityRequest.Pax pax = new HotelBedsV2AvailabilityRequest.Pax();
                    pax.age = 30;
                    pax.type = "AD";
                    paxList.Add(pax);
                }
                for (int infantIncrementer = 1; infantIncrementer <= roomDetail.Infants; infantIncrementer++)
                {
                    HotelBedsV2AvailabilityRequest.Pax pax = new HotelBedsV2AvailabilityRequest.Pax();
                    pax.age = 1;
                    pax.type = "CH";
                    paxList.Add(pax);
                }

                occupancy.paxes = paxList.ToArray();

                occupancies.Add(occupancy);
            }

            HotelBedsV2AvailabilityRequest.Hotels hotels = new HotelBedsV2AvailabilityRequest.Hotels();

            hotels.hotel = hotelIDList.ToArray();
            if (hotelIDList.Any())
            {
                hotelBedsV2AvailabilityRequest.hotels = hotels;
            }

            HotelBedsV2AvailabilityRequest.GeoLocation geoLocation = new HotelBedsV2AvailabilityRequest.GeoLocation();
            if (searchDetails.Radius > 0)
            {
                geoLocation.Latitude = searchDetails.Latitude;
                geoLocation.Longitude = searchDetails.Longitude;
                geoLocation.radius = searchDetails.Radius;
                hotelBedsV2AvailabilityRequest.geoLocation = geoLocation;
            }

            var UseDestination = (!hotelIDList.Any()) && !(searchDetails.Radius > 0);

            HotelBedsV2AvailabilityRequest.Boards boards = new HotelBedsV2AvailabilityRequest.Boards();
            if (searchDetails.MealBasisID > 0)
            {
                boards.included = true;
                List<string> mealBasisList = new List<string>();

                mealBasisList.Add(_support.TPMealBasisLookup(Source, searchDetails.MealBasisID));
                boards.board = mealBasisList.ToArray();
            }

            HotelBedsV2AvailabilityRequest.Filter filter = new HotelBedsV2AvailabilityRequest.Filter();
            filter.packaging = _settings.Packaging(searchDetails);
            if (!_settings.AllowAtHotelPayments(searchDetails))
            {
                filter.paymentType = "AT_WEB";
            }
            filter.maxRate = 1000000;
            filter.maxCategory = 5;
            filter.minCategory = 1;
            filter.maxRatesPerRoom = 1000000;

            string hotelPackage;

            if (_settings.HotelPackage(searchDetails))
            {
                hotelPackage = "BOTH";
            }
            else
            {
                hotelPackage = "NO";
            }

            filter.hotelPackage = hotelPackage;

            hotelBedsV2AvailabilityRequest.destination = new HotelBedsV2AvailabilityRequest.Destination();

            hotelBedsV2AvailabilityRequest.occupancies = occupancies.ToArray();
            hotelBedsV2AvailabilityRequest.stay = stay;
            hotelBedsV2AvailabilityRequest.boards = boards;
            hotelBedsV2AvailabilityRequest.filter = filter;

            return hotelBedsV2AvailabilityRequest;
        }
        private string RemoveBadRequestElements(string request)
        {
            request = request.Replace(",\"destination\":{\"code\":null}", "");
            request = request.Replace(",\"geoLocation\":null", "");
            request = request.Replace(",\"boards\":{\"included\":false,\"board\":null}", "");
            request = request.Replace(",\"rooms\":null", "");
            request = request.Replace(",\"keywords\":null", "");
            request = request.Replace(",\"reviews\":null", "");
            request = request.Replace(",\"accommodations\":null", "");
            request = request.Replace(",\"hotels\":null", "");
            return request;
        }

        public static string GetSignature(string user, string password)
        {
            var utcDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var assemble = user + password + utcDate.ToString();
            var hashStringBuilder = new System.Text.StringBuilder();
            using (var sha256 = new System.Security.Cryptography.SHA256Managed())
            {

                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(assemble)).ToList();
                foreach (var b in hashedBytes)
                {
                    hashStringBuilder.Append(b.ToString("x2"));

                }
            }
            return hashStringBuilder.ToString();
        }
    }
    public class SearchHelper : SearchExtraHelper
    {
        public ResortSplit ResortSplit { get; set; } = new();
        public List<string> PropertyIDs { get; } = new List<string>();
    }
}