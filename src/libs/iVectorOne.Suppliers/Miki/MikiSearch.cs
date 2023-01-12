namespace iVectorOne.Suppliers.Miki
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Caching.Memory;
    using Models;
    using Models.Common;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using Result = Models.Common.Result;
    using RoomDetail = iVector.Search.Property.RoomDetail;
    using iVectorOne.Models.Property;

    public class MikiSearch : IThirdPartySearch, ISingleSource
    {
        private readonly IMikiSettings _settings;
        private readonly ITPSupport _support;
        private readonly ISerializer _serializer;
        private readonly IMemoryCache _cache;

        public MikiSearch(IMikiSettings settings, ITPSupport support, ISerializer serializer, IMemoryCache cache)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _cache = Ensure.IsNotNull(cache, nameof(cache));
        }

        public string Source => ThirdParties.MIKI;

        public async Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requestXml = await GetXmlAsync(searchDetails, resortSplits);

            var request = new Request
            {
                EndPoint = _settings.BaseURL(searchDetails),
                Method = RequestMethod.POST,
                ContentType = "application/soap+xml;charset=UTF-8;action=\"hotelSearch\""
            };
            request.SetRequest(requestXml);

            return new List<Request> { request };
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformed = new TransformedResultCollection();

            var response = _serializer.DeSerialize<Envelope<HotelSearchResponse>>(requests[0].ResponseXML);

            // do we have any results or errors?
            if (response.Body.Content.Errors.Length > 0 || response.Body.Content.Hotels.Length == 0)
            {
                return transformed;
            }

            var result = new Result
            {
                Response = response,
                Properties = BuildRooms(response, searchDetails),
                RoomDescriptions = response.Body.Content.Hotels
                    .SelectMany(x => x.RoomOptions)
                    .Select(x => x.RoomTypeCode)
                    .Distinct()
                    .Where(roomTypeCode => !string.IsNullOrEmpty(MikiSupport.GetRoomTypeInfoAsync(roomTypeCode, _cache).Result)) // todo - async
                    .Select(roomTypeCode => new RoomDescription
                    {
                        RoomTypeCode = roomTypeCode,
                        Description = MikiSupport.GetRoomDescriptionAsync(_settings.Language(searchDetails), roomTypeCode, _cache).Result // todo - async
                    })
                    .ToList()
            };
            
            transformed.TransformedResults.Add(GetResultFromResponse(result));

            return transformed;
        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            // 1. no more then 2 kids per room
            bool hasSearchRestrictions = searchDetails.RoomDetails.Any(room => room.Infants + room.Children > 2);

            // 2. booking no longer than 21 nights
            if (!hasSearchRestrictions)
            {
                if (searchDetails.Duration > 21)
                    hasSearchRestrictions = true;
            }

            // 3. don't send 0 nights
            if (!hasSearchRestrictions)
            {
                if (searchDetails.Duration == 0)
                    hasSearchRestrictions = true;
            }

            // 4. No more than 5 rooms
            if (!hasSearchRestrictions)
            {
                if (searchDetails.RoomDetails.Count > 5)
                    hasSearchRestrictions = true;
            }

            return hasSearchRestrictions;
        }

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        public TransformedResult GetResultFromResponse(Result result)
        {
            var transformedResult = new TransformedResult();
            var hotelSearchResponse = result.Response.Body.Content;
            var currency = hotelSearchResponse.Hotels.Select(x => x.CurrencyCode).First();
            var agentCode = hotelSearchResponse.ResponseAuditInfo.AgentCode;

            foreach (var hotel in result.Response.Body.Content.Hotels)
            {
                string productCode = hotel.ProductCode;
                foreach (var room in result.Properties.Property
                             .Where(x => x.ProductCode == productCode)
                             .SelectMany(x => x.Rooms))
                {
                    foreach (var roomType in room.RoomTypes)
                    {
                        foreach (var roomOption in result.Response.Body.Content.Hotels
                                     .Where(x => x.ProductCode == productCode)
                                     .SelectMany(x => x.RoomOptions)
                                     .Where(r => r.RoomTypeCode == roomType.RoomTypeCode && r.AvailabilityStatus))
                        {
                            string tpReference = $"{roomOption.RoomTypeCode}~{agentCode}~{roomOption.RateIdentifier}";
                            transformedResult = new TransformedResult
                            {
                                TPKey = productCode,
                                CurrencyCode = currency,
                                PropertyRoomBookingID = room.Id,
                                RoomType = roomOption.RoomDescription,
                                MealBasisCode = roomOption.MealBasis.MealBasisCode,
                                Amount = roomOption.RoomTotalPrice.Price,
                                TPReference = tpReference
                            };
                        }
                    }
                }
            }

            return transformedResult;
        }

        private Properties BuildRooms(Envelope<HotelSearchResponse> response, SearchDetails searchDetails)
        {
            var properties = new Properties();
            foreach (var propertyNode in response.Body.Content.Hotels)
            {
                var property = new Property { ProductCode = propertyNode.ProductCode };

                int i = 0;
                foreach (var roomDetail in searchDetails.RoomDetails)
                {
                    i++;
                    property.Rooms.Add(new RoomProperty
                    {
                        Id = i,
                        RoomTypes = propertyNode.RoomOptions
                            .Where(x => x.AvailabilityStatus)
                            .Select(r => r.RoomTypeCode)
                            .Distinct()
                            .Where(roomTypeCode => CheckRoomOccupancyAsync(roomTypeCode, roomDetail).Result) // todo - async
                            .Select(roomTypeCode => new RoomType { RoomTypeCode = roomTypeCode })
                            .ToArray()
                    });
                }

                properties.Property.Add(property);
            }

            // currency
            // should be getting currency from response regardless of currency we actually requested results in
            properties.Currency = response.Body.Content.Hotels.Select(x => x.CurrencyCode).First();

            // occupancy
            int index = 0;
            foreach (var room in searchDetails.RoomDetails)
            {
                index++;
                properties.Rooms.Add(new Models.Common.RoomDetail
                {
                    Id = index,
                    Adults = room.Adults,
                    Children = room.Children,
                    Infants = room.Infants,
                    ChildAgeCSV = room.ChildAgeCSV
                });
            }

            return properties;
        }

        private async Task<XmlDocument> GetXmlAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            string agentCode = await MikiSupport.GetAgentCodeAsync(searchDetails, _settings, _support);
            string password = await MikiSupport.GetPasswordAsync(searchDetails, _settings, _serializer, _cache);
            string currencyCode = await MikiSupport.GetCurrencyCodeAsync(searchDetails.ISOCurrencyCode, _support);
            string languageCode = _settings.Language(searchDetails);
            string paxNationality = string.Empty;

            if (!string.IsNullOrEmpty(searchDetails.ISONationalityCode))
            {
                paxNationality = await _support.TPNationalityLookupAsync(ThirdParties.MIKI, searchDetails.ISONationalityCode);
            }

            if (string.IsNullOrEmpty(paxNationality))
            {
                paxNationality = _settings.BookingCountryCode(searchDetails);
            }

            var destination = new Destination();
            if (resortSplits.Sum(o => o.Hotels.Count) <= 50)
            {
                destination.HotelRefs = new HotelRefs
                {
                    ProductCodes = resortSplits.SelectMany(x => x.Hotels).Select(h => h.TPKey).ToArray()
                };
            }
            else
            {
                // get cities and sub locations
                var cities = new Dictionary<int, MikiSupport.City>();
                foreach (var resort in resortSplits)
                {
                    int cityNumber = resort.ResortCode.Split('|')[0].ToSafeInt();
                    int locationId = resort.ResortCode.Split('|')[1].ToSafeInt();

                    if (cityNumber != 0 && cities.ContainsKey(cityNumber))
                    {
                        var city = cities[cityNumber];
                        if (!city.LocationIds.Contains(locationId))
                        {
                            city.LocationIds.Add(locationId);
                        }
                    }
                    else
                    {
                        cities.Add(cityNumber, new MikiSupport.City(cityNumber, locationId));
                    }
                }

                int cityCounter = 1;
                var cityNumbers = new List<int>();
                foreach (var city in cities)
                {
                    if (cityCounter <= 10)
                    {
                        cityNumbers.Add(city.Key);
                        cityCounter++;
                    }
                    else
                    {
                        break;
                    }
                }

                destination.CityNumbers = cityNumbers.ToArray();
            }

            var rooms = new List<Room>();
            int roomCount = 0;
            foreach (var roomDetail in searchDetails.RoomDetails)
            {
                roomCount++;
                var childAgesUnder12 = roomDetail.ChildAges.Where(x => x <= 12).ToList();
                
                var guests = new List<Guest>();
                for (int i = 1; i <= roomDetail.Adults + roomDetail.ChildAges.Count - childAgesUnder12.Count; i++)
                {
                    guests.Add(new Guest{ Type = GuestCountType.ADT });
                }

                if (childAgesUnder12.Count + roomDetail.Infants > 0)
                {
                    for (int i = 1; i <= childAgesUnder12.Count + roomDetail.Infants; i++)
                    {
                        guests.Add(new Guest
                        {
                            Type = GuestCountType.CHD,
                            Age = childAgesUnder12.Count > i - 1 
                                ? Math.Max(childAgesUnder12[i - 1], 3) 
                                : 3
                        });
                    }
                }

                rooms.Add(new Room
                {
                    RoomNo = roomCount,
                    Guests = guests.ToArray()
                });
            }

            var request = new Envelope<HotelSearchRequest>
            {
                Body =
                {
                    Content =
                    {
                        VersionNumber = "7.0",
                        RequestAuditInfo = MikiSupport.BuildRequestAuditInfo(agentCode, password),
                        HotelSearchCriteria =
                        {
                            CurrencyCode = currencyCode,
                            PaxNationality = paxNationality,
                            LanguageCode = languageCode,
                            StayPeriod =
                            {
                                CheckinDate = searchDetails.ArrivalDate.ToString("yyyy-MM-dd"),
                                NumberOfNights = searchDetails.Duration
                            },
                            Rooms = rooms.ToArray(),
                            Destination = destination
                        },
                    }
                }
            };

            return _serializer.Serialize(request);
        }

        private async Task<bool> CheckRoomOccupancyAsync(string roomTypeCode, RoomDetail room)
        {
            // check child ages
            int adults = room.Adults;
            int children = room.Children;
            int infants = room.Infants;

            foreach (int childAge in room.ChildAges)
            {
                switch (childAge)
                {
                    case > 12:
                        children -= 1;
                        adults += 1;
                        break;
                    case < 3:
                        children -= 1;
                        infants += 1;
                        break;
                }
            }

            string roomTypeInfo = await MikiSupport.GetRoomTypeInfoAsync(roomTypeCode, _cache);
            if (string.IsNullOrEmpty(roomTypeInfo)) return false;

            // get occ rules
            var occupancyRules = new OccupancyRules(roomTypeInfo);

            // temp - treating infants as children
            children += infants;
            infants = 0;

            // infants don't count toward max pax
            // children only count toward max pax if description does not contain 'extra bed'
            int totalPax = adults;
            if (!roomTypeInfo.Split('|')[1].Contains("Extra Bed"))
            {
                totalPax += children;
            }

            if ((totalPax <= occupancyRules.MaxPax && totalPax >= occupancyRules.MinPax)
                && (adults <= occupancyRules.MaxAdults && adults >= occupancyRules.MinAdults)
                && (children <= occupancyRules.MaxChildren && children >= occupancyRules.MinChildren)
                && (infants <= occupancyRules.MaxInfants && infants >= occupancyRules.MinInfants))
            {
                return true;
            }

            return false;
        }

        private class OccupancyRules
        {
            public int MaxPax;
            public int MinPax;
            public int MaxAdults;
            public int MinAdults;
            public int MaxChildren;
            public int MinChildren;
            public int MaxInfants;
            public int MinInfants;

            public OccupancyRules(string roomTypeInfo)
            {
                string[] roomTypeInfoSplit = MikiSupport.GetSplit(roomTypeInfo);
                MaxPax = roomTypeInfoSplit[2].ToSafeInt();
                MinPax = roomTypeInfoSplit[3].ToSafeInt();
                MaxAdults = roomTypeInfoSplit[4].ToSafeInt();
                MinAdults = roomTypeInfoSplit[5].ToSafeInt();
                MaxChildren = roomTypeInfoSplit[6].ToSafeInt();
                MinChildren = roomTypeInfoSplit[7].ToSafeInt();
                MaxInfants = roomTypeInfoSplit[8].ToSafeInt();
                MinInfants = roomTypeInfoSplit[9].ToSafeInt();
            }
        }

    }
}
