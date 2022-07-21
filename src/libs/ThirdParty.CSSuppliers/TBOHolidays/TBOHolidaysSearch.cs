namespace ThirdParty.CSSuppliers.TBOHolidays
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using Lookups;
    using Models;
    using Models.Common;
    using ThirdParty.Constants;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Results.Models;

    public class TBOHolidaysSearch : IThirdPartySearch, ISingleSource
    {
        private const string DateFormat = "yyyy-MM-ddThh:mm:ss";

        private readonly ITBOHolidaysSettings _settings;
        private readonly ITPSupport _support;
        private readonly ISerializer _serializer;

        public TBOHolidaysSearch(ITBOHolidaysSettings settings, ITPSupport support, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        public string Source => ThirdParties.TBOHOLIDAYS;

        public async Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = new List<Request>();

            foreach (var resortSplit in resortSplits)
            {
                var request = await BuildSearchRequestAsync(searchDetails, "HotelSearchWithRooms", resortSplit);

                var webRequest = new Request
                {
                    UseGZip = true,
                    EndPoint = _settings.URL(searchDetails),
                    Method = RequestMethod.POST,
                    ContentType = ContentTypes.Application_SOAP_XML,
                };

                webRequest.SetRequest(_serializer.Serialize(request));
                requests.Add(webRequest);
            }

            return requests;
        }

        private async Task<Envelope<HotelSearchWithRoomsRequest>> BuildSearchRequestAsync(SearchDetails searchDetails, string type, ResortSplit resortSplit)
        {
            string[] resortSplitItems = resortSplit.ResortCode.Split('|');
            string guestNationality = string.Empty;

            if (!string.IsNullOrEmpty(searchDetails.ISONationalityCode))
            {
                guestNationality = await _support.TPNationalityLookupAsync(Source, searchDetails.ISONationalityCode);
            }

            if (string.IsNullOrEmpty(guestNationality))
            {
                guestNationality = _settings.DefaultGuestNationality(searchDetails);
            }

            var envelope = new Envelope<HotelSearchWithRoomsRequest>
            {
                Header = TBOHolidays.BuildHeader(type, searchDetails, _settings),
                Body =
                {
                    Content =
                    {
                        CheckInDate = searchDetails.ArrivalDate.ToString(DateFormat),
                        CheckOutDate = searchDetails.DepartureDate.ToString(DateFormat),
                        CountryName = resortSplitItems[0],
                        CityName = resortSplitItems[1],
                        CityId = resortSplitItems[2],
                        IsNearBySearchAllowed = "false",
                        NoOfRooms = searchDetails.RoomDetails.Count,
                        GuestNationality = guestNationality,
                        PreferredCurrencyCode = _settings.CurrencyCode(searchDetails),
                        ResultCount = _settings.ResultCount(searchDetails).ToSafeString()
                    }
                }
            };

            var request = envelope.Body.Content;

            foreach (var roomDetails in searchDetails.RoomDetails)
            {
                var roomGuest = new RoomGuest
                {
                    AdultCount = roomDetails.Adults,
                    ChildCount = roomDetails.Children + roomDetails.Infants
                };

                // don't have ages so input dummy
                if (roomGuest.ChildCount > 0)
                {
                    foreach (int childAge in roomDetails.ChildAges)
                    {
                        roomGuest.ChildAge.Add(childAge);
                    }

                    for (int i = 1; i <= roomDetails.Infants; i++)
                    {
                        roomGuest.ChildAge.Add(0);
                    }
                }

                request.RoomGuests.Add(roomGuest);
            }

            return envelope;
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformed = new TransformedResultCollection();

            var hotelResponse = new HotelResponse
            {
                Envelopes = requests
                    .Select(x => _serializer.DeSerialize<Envelope<HotelSearchWithRoomsResponse>>(x.ResponseXML))
                    .ToArray()
            };

            // dedupe properties as results from multiple requests can return the same property
            foreach (var response in hotelResponse.Envelopes)
            {
                var resultList = new List<HotelResult>();

                foreach (var hotelResult in response.Body.Content.HotelResultList)
                {
                    string code = hotelResult.HotelInfo.HotelCode;
                    if (resultList.All(x => x.HotelInfo.HotelCode != code))
                    {
                        resultList.Add(hotelResult);
                    }
                }

                response.Body.Content.HotelResultList = resultList.ToArray();
            }

            var combination = FindPossibleCombinations(hotelResponse, searchDetails);
            var results = new Results { HotelResponse = hotelResponse, HotelCombinations = combination };

            bool multiRoom = searchDetails.RoomDetails.Count > 1; 

            transformed.TransformedResults.AddRange(BuildTransformResult(results, multiRoom));
            return transformed;
        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return false;
        }

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        public HotelCombinations FindPossibleCombinations(HotelResponse response, SearchDetails searchDetails)
        {
            // new hotel
            var hotelCombinations = new List<HotelCombination>();
            var hotelCodes = new List<string>();

            //gather the raw room information
            foreach (var hotelNode in response.Envelopes.SelectMany(x => x.Body.Content.HotelResultList))
            {
                var hotel = new Hotel { HotelCode = hotelNode.HotelInfo.HotelCode };
                
                for (int prbid = 1; prbid <= searchDetails.RoomDetails.Count; prbid++)
                {
                    var hotelRoom = new Room
                    {
                        PRBID = prbid
                    };

                    foreach (var roomCombination in hotelNode.OptionsForBooking.RoomCombination)
                    {
                        int roomIndex = roomCombination.RoomIndex[prbid - 1];

                        if (!hotelRoom.RoomIndex.Contains(roomIndex))
                        {
                            hotelRoom.RoomIndex.Add(roomIndex);
                        }
                    }

                    hotel.Rooms.Add(hotelRoom);
                }

                // process - if total combinations match then display all, else only display first
                int totalCombinations = hotelNode.OptionsForBooking.RoomCombination.Length;
                
                int totalPossibleCombinations = hotel.Rooms.First(x => x.PRBID == 1).RoomIndex.Count;
                totalPossibleCombinations = hotel.Rooms
                    .Where(x => x.PRBID != 1)
                    .Aggregate(totalPossibleCombinations, (current, room) =>
                        current * room.RoomIndex.Count);

                string combinationType = totalCombinations == totalPossibleCombinations 
                    ? "All" 
                    : "First";

                if (!hotelCodes.Contains(hotel.HotelCode))
                {
                    hotelCombinations.Add(new HotelCombination
                    {
                        HotelCode = hotel.HotelCode,
                        CombinationType = combinationType,
                        Rooms = combinationType == "All" 
                            ? hotel.Rooms.Select(x => new Models.Common.Room
                            {
                                PRBID = x.PRBID,
                                RoomIndex = x.RoomIndex.ToArray(),
                            }).ToArray() 
                            : Array.Empty<Models.Common.Room>()
                    });

                    hotelCodes.Add(hotel.HotelCode);
                }
            }

            return new HotelCombinations { HotelCombination = hotelCombinations.ToArray() };
        }

        private List<TransformedResult> BuildTransformResult(Results result, bool multiRoom)
        {
            var transformedResult = new List<TransformedResult>();

            foreach (var response in result.HotelResponse.Envelopes)
            {
                var searchResponse = response.Body.Content;

                foreach (var hotelCombination in result.HotelCombinations.HotelCombination)
                {
                    int resultIndex = searchResponse.HotelResultList
                        .First(x => x.HotelInfo.HotelCode == hotelCombination.HotelCode).ResultIndex;

                    if (!multiRoom)
                    {
                        foreach (var hotelRoom in searchResponse.HotelResultList
                                     .Where(r => r.HotelInfo.HotelCode == hotelCombination.HotelCode)
                                     .SelectMany(x => x.HotelRooms))
                        {
                            transformedResult.Add(GetTransformedResultModel(
                                hotelRoom,
                                hotelCombination.HotelCode,
                                resultIndex,
                                searchResponse.SessionId,
                                1));
                        }
                    }
                    else
                    {
                        if (hotelCombination.CombinationType == "First")
                        {
                            int position = 1;

                            var hotelResult = searchResponse.HotelResultList
                                .First(x => x.HotelInfo.HotelCode == hotelCombination.HotelCode);

                            foreach (int roomIndex in hotelResult.OptionsForBooking.RoomCombination[0].RoomIndex)
                            {
                                var hotelRoom = hotelResult.HotelRooms
                                    .First(x => x.RoomIndex == roomIndex);

                                transformedResult.Add(GetTransformedResultModel(
                                    hotelRoom,
                                    hotelCombination.HotelCode,
                                    resultIndex,
                                    searchResponse.SessionId,
                                    position));

                                position++;
                            }
                        }

                        if (hotelCombination.CombinationType == "All")
                        {
                            foreach (var room in hotelCombination.Rooms)
                            {
                                foreach (int roomIndex in room.RoomIndex)
                                {
                                    var hotelRoom = searchResponse.HotelResultList
                                        .Where(x => x.HotelInfo.HotelCode == hotelCombination.HotelCode)
                                        .SelectMany(h => h.HotelRooms)
                                        .First(x => x.RoomIndex == roomIndex);

                                    transformedResult.Add(GetTransformedResultModel(
                                        hotelRoom,
                                        hotelCombination.HotelCode,
                                        resultIndex,
                                        searchResponse.SessionId,
                                        room.PRBID));
                                }
                            }
                        }
                    }
                }
            }

            return transformedResult;
        }

        private static TransformedResult GetTransformedResultModel(
            HotelRoom hotelRoom,
            string hotelCode,
            int resultIndex,
            string sessionId,
            int prbId)
        {
            var rate = hotelRoom.RoomRate;

            string supplementInformation = string.Concat(hotelRoom.Supplements.Supplement
                .Where(x => x.SuppChargeType == SuppChargeType.AtProperty && x.SuppIsMandatory)
                .Select(c => $"{c.SuppID},{c.Price}|"));

            string roomRateElement = $"{rate.B2CRates},{rate.AgentMarkUp},{rate.RoomTax},{rate.RoomFare},{rate.Currency},{rate.TotalFare}";
            string tpReference = string.Concat(
                sessionId,
                Helper.Separators[0],
                resultIndex,
                Helper.Separators[0],
                hotelRoom.RoomIndex,
                Helper.Separators[0],
                hotelRoom.RoomTypeCode,
                Helper.Separators[0],
                hotelRoom.RatePlanCode,
                Helper.Separators[0],
                roomRateElement,
                Helper.Separators[0],
                supplementInformation);

            return new TransformedResult
            {
                TPKey = hotelCode,
                CurrencyCode = rate.Currency,
                PropertyRoomBookingID = prbId,
                RoomType = hotelRoom.RoomTypeName,
                MealBasisCode = "RO",
                Amount = rate.TotalFare,
                TPReference = tpReference,
                Discount = hotelRoom.Discount
            };
        }

        private class Hotel
        {
            public string HotelCode { get; set; } = string.Empty;
            public List<Room> Rooms { get; set; } = new();
        }

        private class Room
        {
            public int PRBID { get; set; }
            public List<int> RoomIndex { get; set; } = new();
        }
    }
}