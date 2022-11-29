namespace iVectorOne.Suppliers.TBOHolidays
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using Lookups;
    using Models;
    using Models.Common;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using iVectorOne.SDK.V2.PropertySearch;
    using Newtonsoft.Json;
    

    public class TBOHolidaysSearch : IThirdPartySearch, ISingleSource
    {
        private const string DateFormat = "yyyy-MM-dd";

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

        public async Task<List<Intuitive.Helpers.Net.Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            if (resortSplits.Count == 0)
            {
                return new List<Intuitive.Helpers.Net.Request>();
            }

            var requests = new List<Intuitive.Helpers.Net.Request>();

            for (var index = 0; index < searchDetails.Rooms; index++)
            {
                var request = await BuildSearchRequestAsync(searchDetails, resortSplits, index);
                
                var webRequest = new Intuitive.Helpers.Net.Request
                {
                    EndPoint = /*_settings.SearchURL(searchDetails)*/"http://api.tbotechnology.in/TBOHolidays_HotelAPI/Search",
                    Method = RequestMethod.POST,
                    ContentType = ContentTypes.Application_json,
                    Accept = "application/json",
                };

                webRequest.Headers.AddNew("Authorization", "Basic " + "QW5keXN5c1Rlc3Q6SW50QDIwODM3Mzg1");
                webRequest.SetRequest(request);
                requests.Add(webRequest);
            }

            return requests;
        }

        private async Task<string> BuildSearchRequestAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits, int index)
        {
            var guestNationality = string.Empty;

            if (!string.IsNullOrEmpty(searchDetails.ISONationalityCode))
            {
                guestNationality = await _support.TPNationalityLookupAsync(Source, searchDetails.ISONationalityCode);
            }

            if (string.IsNullOrEmpty(guestNationality))
            {
                guestNationality = _settings.LeadGuestNationality(searchDetails);
            }

            var request = new HotelSearchWithRoomsRequest
            {
                CheckIn = searchDetails.ArrivalDate.ToString(DateFormat),
                CheckOut = searchDetails.DepartureDate.ToString(DateFormat),
                HotelCodes = string.Join(',', resortSplits.SelectMany(x => x.Hotels).Select(x => x.TPKey)),
                GuestNationality = guestNationality,
                ResponseTime = 23.0m,
                IsDetailedResponse = false,
                Filters =
                {
                    Refundable = /*_settings.ExcludeNRF(searchDetails)*/ false,
                    NoOfRooms = 0,
                    MealType = /*_settings.RequestedMealBases(searchDetails)*/ "All"
                }
            };

            var roomDetail = searchDetails.RoomDetails[index];
            var paxRoom = new PaxRoom
            {
                Adults = roomDetail.Adults,
                Children = roomDetail.Children + roomDetail.Infants,
            };

            if (paxRoom.Children > 0)
            {
                paxRoom.ChildrenAges.AddRange(roomDetail.ChildAges);
                for (var i = 0; i < roomDetail.Infants; i++)
                {
                    paxRoom.ChildrenAges.Add(0);
                }
            }

            request.PaxRooms.Add(paxRoom);
            
            return JsonConvert.SerializeObject(request);
        }

        public TransformedResultCollection TransformResponse(List<Intuitive.Helpers.Net.Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformed = new TransformedResultCollection();

            var hotelResponses = requests
                .Select(request => JsonConvert.DeserializeObject<HotelResponse>(request.ResponseString))
                .Where(x => x.Status.Code == "200" && x.Status.Description == "Successful")
                .ToList();

            if (hotelResponses.Count != searchDetails.Rooms)
            {
                return transformed;
            }

            transformed.TransformedResults.AddRange(BuildTransformResult(hotelResponses!));
            return transformed;
        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return false;
        }

        public bool ResponseHasExceptions(Intuitive.Helpers.Net.Request request)
        {
            return false;
        }

        #region OLD

        //public HotelCombinations FindPossibleCombinations(List<HotelResponse> responses, SearchDetails searchDetails)
        //{
        //    // new hotel
        //    var hotelCombinations = new List<HotelCombination>();
        //    var hotelCodes = new List<string>();

        //    //gather the raw room information
        //    foreach (var hotelNode in responses.SelectMany(x => x.HotelResult))
        //    {
        //        var hotel = new Hotel { HotelCode = hotelNode.HotelCode };

        //        for (int prbid = 1; prbid <= searchDetails.RoomDetails.Count; prbid++)
        //        {
        //            var hotelRoom = new Room
        //            {
        //                PRBID = prbid
        //            };

        //            //foreach (var roomCombination in hotelNode.OptionsForBooking.RoomCombination)
        //            //{
        //            //    int roomIndex = roomCombination.RoomIndex[prbid - 1];

        //            //    if (!hotelRoom.RoomIndex.Contains(roomIndex))
        //            //    {
        //            //        hotelRoom.RoomIndex.Add(roomIndex);
        //            //    }
        //            //}

        //            hotel.Rooms.Add(hotelRoom);
        //        }

        //        // process - if total combinations match then display all, else only display first
        //        int totalCombinations = hotelNode.OptionsForBooking.RoomCombination.Length;

        //        int totalPossibleCombinations = hotel.Rooms.First(x => x.PRBID == 1).RoomIndex.Count;
        //        totalPossibleCombinations = hotel.Rooms
        //            .Where(x => x.PRBID != 1)
        //            .Aggregate(totalPossibleCombinations, (current, room) =>
        //                current * room.RoomIndex.Count);

        //        string combinationType = totalCombinations == totalPossibleCombinations 
        //            ? "All" 
        //            : "First";

        //        if (!hotelCodes.Contains(hotel.HotelCode))
        //        {
        //            hotelCombinations.Add(new HotelCombination
        //            {
        //                HotelCode = hotel.HotelCode,
        //                CombinationType = combinationType,
        //                Rooms = combinationType == "All" 
        //                    ? hotel.Rooms.Select(x => new Models.Common.Room
        //                    {
        //                        PRBID = x.PRBID,
        //                        RoomIndex = x.RoomIndex.ToArray(),
        //                    }).ToArray() 
        //                    : Array.Empty<Models.Common.Room>()
        //            });

        //            hotelCodes.Add(hotel.HotelCode);
        //        }
        //    }

        //    return new HotelCombinations { HotelCombination = hotelCombinations.ToArray() };
        //}

        //public HotelCombinations FindPossibleCombinations(List<HotelResponse> responses, SearchDetails searchDetails)
        //{
        //    // new hotel
        //    var hotelCombinations = new List<HotelCombination>();
        //    var hotelCodes = new List<string>();

        //    //gather the raw room information
        //    foreach (var hotelNode in responses[0].HotelResult)
        //    {
        //        if (!responses.All(x => x.HotelResult.Any(hc => hc.HotelCode == hotelNode.HotelCode)))
        //        {
        //            continue;
        //        }

        //        var hotel = new Hotel { HotelCode = hotelNode.HotelCode };

        //        var limit = responses
        //            .SelectMany(x => x.HotelResult)
        //            .Where(h => h.HotelCode == hotel.HotelCode)
        //            .OrderBy(r => r.Rooms.Length)
        //            .First().Rooms.Length;

        //        for (var index = 0; index < limit; index++)
        //        {
        //            for (var prbid = 0; prbid < searchDetails.RoomDetails.Count; prbid++)
        //            {
        //                var hotelRoom = new Room
        //                {
        //                    PRBID = prbid + 1
        //                };

        //                var roomIndex = responses[prbid].HotelResult
        //                    .First(x => x.HotelCode == hotel.HotelCode)
        //                    .Rooms[index]
        //                    .BookingCode.Split('!')[2]
        //                    .ToSafeInt();


        //            }
        //        }
        //    }


        //    foreach (var hotelNode in responses.SelectMany(x => x.HotelResult))
        //    {
        //        var hotel = new Hotel { HotelCode = hotelNode.HotelCode };

        //        for (int prbid = 1; prbid <= searchDetails.RoomDetails.Count; prbid++)
        //        {
        //            var hotelRoom = new Room
        //            {
        //                PRBID = prbid
        //            };

        //            //foreach (var roomCombination in hotelNode.OptionsForBooking.RoomCombination)
        //            //{
        //            //    int roomIndex = roomCombination.RoomIndex[prbid - 1];

        //            //    if (!hotelRoom.RoomIndex.Contains(roomIndex))
        //            //    {
        //            //        hotelRoom.RoomIndex.Add(roomIndex);
        //            //    }
        //            //}

        //            hotel.Rooms.Add(hotelRoom);
        //        }

        //        // process - if total combinations match then display all, else only display first
        //        int totalCombinations = hotelNode.OptionsForBooking.RoomCombination.Length;

        //        int totalPossibleCombinations = hotel.Rooms.First(x => x.PRBID == 1).RoomIndex.Count;
        //        totalPossibleCombinations = hotel.Rooms
        //            .Where(x => x.PRBID != 1)
        //            .Aggregate(totalPossibleCombinations, (current, room) =>
        //                current * room.RoomIndex.Count);

        //        string combinationType = totalCombinations == totalPossibleCombinations
        //            ? "All"
        //            : "First";

        //        if (!hotelCodes.Contains(hotel.HotelCode))
        //        {
        //            hotelCombinations.Add(new HotelCombination
        //            {
        //                HotelCode = hotel.HotelCode,
        //                CombinationType = combinationType,
        //                Rooms = combinationType == "All"
        //                    ? hotel.Rooms.Select(x => new Models.Common.Room
        //                    {
        //                        PRBID = x.PRBID,
        //                        RoomIndex = x.RoomIndex.ToArray(),
        //                    }).ToArray()
        //                    : Array.Empty<Models.Common.Room>()
        //            });

        //            hotelCodes.Add(hotel.HotelCode);
        //        }
        //    }

        //    return new HotelCombinations { HotelCombination = hotelCombinations.ToArray() };
        //}

        #endregion

        private List<TransformedResult> BuildTransformResult(List<HotelResponse> result)
        {
            var transformedResult = new List<TransformedResult>();

            foreach (var hotelCode in result
                         .SelectMany(x => x.HotelResult)
                         .GroupBy(hr => hr.HotelCode)
                         .Where(x => x.Count() == result.Count)
                         .Select(g => g.Key))
            {
                for (var prbid = 0; prbid < result.Count; prbid++)
                {
                    foreach (var hotelResult in result[prbid].HotelResult.Where(hr => hr.HotelCode == hotelCode))
                    {
                        foreach (var room in hotelResult.Rooms)
                        {
                            var supplements = room.Supplements
                                    .SelectMany(x => x.Where(s => s.Type == SuppChargeType.AtProperty)
                                    .ToList());

                            var supplementInformation = string.Concat(supplements.Select(c => $"{c.Index},{c.Price}|"));
                            var tpReference = string.Concat(room.BookingCode, Helper.Separators[0],
                                supplementInformation);

                            transformedResult.Add(new TransformedResult
                            {
                                TPKey = hotelResult.HotelCode,
                                CurrencyCode = hotelResult.Currency,
                                PropertyRoomBookingID = prbid + 1,
                                RoomType = room.Name.First(),
                                MealBasisCode = room.MealType,
                                Amount = room.TotalFare,
                                TPReference = tpReference,
                                SellingPrice = room.RecommendedSellingRate,
                                Adjustments = supplements
                                    .Select(s => new TransformedResultAdjustment(
                                        AdjustmentType.Supplement,
                                        string.Empty,
                                        s.Description,
                                        s.Price)).ToList()
                            });
                        }
                    }
                }
            }

            return transformedResult;
        }

        //private class Hotel
        //{
        //    public string HotelCode { get; set; } = string.Empty;
        //    public List<Room> Rooms { get; set; } = new();
        //}

        //private class Room
        //{
        //    public int PRBID { get; set; }
        //    public List<int> RoomIndex { get; set; } = new();
        //}
    }
}