namespace iVectorOne.Suppliers.ATI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using iVector.Search.Property;
    using Microsoft.Extensions.Caching.Memory;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.ATI.Models;
    using iVectorOne.Suppliers.ATI.Models.Common;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;

    public class ATISearch : IThirdPartySearch, ISingleSource
    {
        private readonly IATISettings _settings;
        private readonly ISerializer _serializer;
        private readonly IMemoryCache _cache;
        
        public ATISearch(IATISettings settings, ISerializer serializer, IMemoryCache cache)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _cache = Ensure.IsNotNull(cache, nameof(cache));
        }

        public string Source => ThirdParties.ATI;

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            string userID = _settings.User(searchDetails);
            string version = _settings.Version(searchDetails, false);
            string password = _settings.Password(searchDetails);
            string url = _settings.GenericURL(searchDetails);

            var requestXml = _serializer.Serialize(
                GetSearchRequestXml(
                    searchDetails.ArrivalDate,
                    searchDetails.Duration,
                    searchDetails.RoomDetails,
                    GetCriteria(resortSplits).ToList(),
                    version,
                    userID));

            var request = new Request
            {
                UserName = userID,
                Password = password,
                AuthenticationMode = AuthenticationMode.Basic,
                EndPoint = url,
                Method = RequestMethod.POST,
                ExtraInfo = searchDetails,
                SOAP = true,
                UseGZip = _settings.UseGZip(searchDetails)
            };
            request.SetRequest(requestXml);

            return Task.FromResult(new List<Request> { request });
        }

        public static Envelope<AtiAvailabilityRequest> GetSearchRequestXml(
            DateTime arrivalDate,
            int duration,
            RoomDetails roomDetails,
            List<Criterion> criteria,
            string version,
            string userId)
        {
            var request = new Envelope<AtiAvailabilityRequest>();
            request.Body.Content = new AtiAvailabilityRequest
            {
                Version = version,
                Pos = new Pos { Source = new Source { UserId = userId } },
                AvailRequestSegments = new[]{ new AvailRequestSegment
                {
                    StayDateRange = new StayDateRange
                    {
                        Start = arrivalDate.ToString("yyyy-MM-dd"),
                        Duration = "P0Y0M" + duration + "D",
                    },
                    RoomStayCandidates = roomDetails.Select(room =>
                    {
                        var guestCounts = new List<GuestCount>
                        {
                            new() { AgeQualifyingCode = "10", Count = room.Adults }
                        };

                        guestCounts.AddRange(room
                            .ChildAndInfantAges(1)
                            .Select(childAge => new GuestCount { AgeQualifyingCode = "08", Age = childAge, Count = 1 }));

                        return new RoomStayCandidate { GuestCounts = guestCounts.ToArray() };
                    }).ToArray(),
                    HotelSearchCriteria = new HotelSearchCriteria { Criterion = criteria.ToArray() },
                }}
            };

            return request;
        }

        private static IEnumerable<Criterion> GetCriteria(List<ResortSplit> resortSplits)
        {
            if (resortSplits.Count == 1 && resortSplits[0].Hotels.Count == 1)
            {
                yield return new Criterion { HotelRef = new HotelRef { HotelCode = resortSplits[0].Hotels[0].TPKey } };
            }
            else
            {
                //resort codes grouped by area code
                foreach (var group in resortSplits.Select(rs => rs.ResortCode).GroupBy(rc => rc.Split('|').First()))
                {
                    // use the area code if there are multiple resorts in the same area
                    // otherwise use the sub metro code
                    string cityCode = group.Count() > 1 ? group.Key : group.First().Split('|').Last();
                    yield return new Criterion { HotelRef = new HotelRef { HotelCityCode = cityCode } };
                }
            }
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformed = new TransformedResultCollection();
            var response = _serializer.DeSerialize<Envelope<AtiAvailabilitySearch>>(requests[0].ResponseXML);

            var mergedResponses = new Results { Envelope = response };

            var groupings = new Groupings();
            foreach (var roomStay in response.Body.Content.RoomStays)
            {
                var grouping = new Grouping();
                try
                {
                    grouping.PropertyCode = roomStay.BasicPropertyInfo.HotelCode;

                    var roomType = roomStay.RoomTypes.First();
                    grouping.RoomTypeCode = roomType.RoomTypeCode.Split('-')[1];
                    grouping.PropertyRoomBooking = roomStay.GuestCounts.First().ResGuestRPH;
                    grouping.RoomTypeDescription = roomType.RoomDescription.Text;

                    if (roomStay.RoomRates.Any(r => r.RatePlanCode == "Available"))
                    {
                        groupings.AddGrouping(grouping);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            mergedResponses.PropertyGroupings = groupings.GetPropertyGroupings(_cache);

            // build occupancy info xml and merge with response xml
            mergedResponses.OccupancyInfo = new OccupancyInfo
            {
                Duration = searchDetails.Duration,
                Rooms = searchDetails.RoomDetails.Select((room, i) => new Room
                {
                    ID = i + 1,
                    Adults = room.Adults,
                    Children = room.Children,
                    Infants = room.Infants,
                    hlpChildAgeCSV = room.ChildAgeCSV,
                }).ToArray()
            };

            transformed.TransformedResults.AddRange(GetResultFromResponse(mergedResponses, searchDetails, _settings));

            return transformed;
        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return searchDetails.Duration > 21;
        }

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        public static List<TransformedResult> GetResultFromResponse(Results mergedResponses, SearchDetails searchDetails, IATISettings settings)
        {
            var transformedResults = new List<TransformedResult>();
            var roomStays = mergedResponses.Envelope.Body.Content.RoomStays;

            foreach (var property in mergedResponses.PropertyGroupings.Properties)
            {
                string currencyCode = roomStays
                    .First(x => x.BasicPropertyInfo.HotelCode == property.PropertyCode).Total.CurrencyCode;

                foreach (var roomType in property.RoomTypes)
                {
                    foreach (var propertyRoomBooking in roomType.PropertyRoomBookings)
                    {
                        var occupancyInfo = mergedResponses.OccupancyInfo.Rooms.First(r => r.ID == propertyRoomBooking.ID);

                        decimal amount = CalculateAmount(mergedResponses, property, propertyRoomBooking, roomType);
                        var adjustments = GetAdjustments(roomStays, propertyRoomBooking, roomType);

                        bool nrf = roomStays
                            .Where(x =>
                            {
                                return x.GuestCounts.Any(g => g.ResGuestRPH == propertyRoomBooking.ID)
                                       && x.RoomTypes.Any(t => t.RoomTypeCode.Substring(t.RoomTypeCode.IndexOf('-') + 1) == roomType.RoomTypeCode);
                            })
                            .SelectMany(x => x.CancelPenalties)
                            .Any(p => p.Deadline.AbsoluteDeadline == "This reservation cannot be cancelled.");

                        var transformedResult =new TransformedResult
                        {
                            TPKey = property.PropertyCode,
                            CurrencyCode = currencyCode,
                            PropertyRoomBookingID = propertyRoomBooking.ID,
                            RoomType = roomType.RoomTypeDescription.Text,
                            RoomTypeCode = $"{property.PropertyCode}-{roomType.RoomTypeCode}",
                            MealBasisCode = "RO",
                            Adults = occupancyInfo.Adults,
                            Children = occupancyInfo.Children,
                            Infants = occupancyInfo.Infants,
                            ChildAgeCSV = occupancyInfo.hlpChildAgeCSV,
                            Amount = amount,
                            TPReference = roomType.RoomTypeCode,
                            NonRefundableRates = nrf,
                            Adjustments = adjustments,
                        };

                        if (settings.ExcludeNRF(searchDetails, false))
                        {
                            if (!transformedResult.NonRefundableRates.GetValueOrDefault())
                            {
                                transformedResults.Add(transformedResult);
                            }
                        }
                        else
                        {
                            transformedResults.Add(transformedResult);
                        }
                    }
                }
            }

            return transformedResults;
        }

        private static decimal CalculateAmount(
            Results mergedResponses,
            Property property,
            PropertyRoomBooking propertyRoomBooking,
            RoomType roomType)
        {
            decimal[] amountCollection = mergedResponses
                .Envelope
                .Body
                .Content
                .RoomStays
                .Where(x => 
                    x.GuestCounts.Any(g => g.ResGuestRPH == propertyRoomBooking.ID) 
                    && x.RoomTypes.Any(t => t.RoomTypeCode == $"{property.PropertyCode}-{roomType.RoomTypeCode}"))
                .SelectMany(rs => rs.RoomRates)
                .SelectMany(r => r.Rates)
                .Select(b => b.Base)
                .Select(a => a.AmountAfterTax)
                .ToArray();

            return amountCollection.Sum() / ((decimal)amountCollection.Length / mergedResponses.OccupancyInfo.Duration) / 100;
        }

        private static List<TransformedResultAdjustment> GetAdjustments(
            RoomStay[] roomStays,
            PropertyRoomBooking propertyRoomBooking,
            RoomType roomType)
        {
           return roomStays
                .Where(r => r.GuestCounts.Any(g => g.ResGuestRPH == propertyRoomBooking.ID)
                            && r.RoomTypes
                                .Any(t => t.RoomTypeCode.Substring(t.RoomTypeCode.IndexOf('-') + 1) == roomType.RoomTypeCode))
                .SelectMany(x => x.RoomRates)
                .SelectMany(r => r.Rates)
                .Select(rate => new TransformedResultAdjustment(
                    SDK.V2.PropertySearch.AdjustmentType.Offer,
                    $"{rate.Discount.DiscountValue} - {rate.EffectiveDate}",
                    $"{rate.Discount.Description}"))
                .ToList();
        }

        public class Groupings : List<Grouping>
        {
            public void AddGrouping(Grouping grouping)
            {
                if (this.Any(existingGroup => existingGroup.PropertyCode == grouping.PropertyCode
                                               && existingGroup.RoomTypeCode == grouping.RoomTypeCode
                                               && existingGroup.PropertyRoomBooking == grouping.PropertyRoomBooking))
                {
                    return;
                }

                Add(grouping);
            }

            private List<string> GetDistinctProperties()
            {
                var result = new List<string>();

                foreach (var grouping in this.Where(grouping => !result.Contains(grouping.PropertyCode)))
                {
                    result.Add(grouping.PropertyCode);
                }

                return result;
            }

            private Dictionary<string, string> GetPropertyRoomTypes(string propertyCode)
            {
                var result = new Dictionary<string, string>();

                foreach (var grouping in this.Where(grouping => 
                             grouping.PropertyCode == propertyCode
                             && !result.ContainsKey(grouping.RoomTypeCode)))
                {
                    result.Add(grouping.RoomTypeCode, grouping.RoomTypeDescription);
                }

                return result;
            }

            private IEnumerable<int> GetPropertyRoomBooking(string propertyCode, string roomTypeCode)
            {
               return this
                   .Where(grouping => grouping.PropertyCode == propertyCode
                                      && grouping.RoomTypeCode == roomTypeCode)
                   .Select(g => g.PropertyRoomBooking)
                   .Distinct();
            }

            private static string GetRoomTypeDescription(string hotelCode, string roomTypeCode, IMemoryCache cache)
            {
                static Dictionary<string, string> cacheBuilder()
                {
                    // read from res file
                    var roomTypes = new StringReader(ATIRes.HotelDescriptions);
                    string roomTypesLine = roomTypes.ReadLine()!;

                    var roomTypeDescriptions = new Dictionary<string, string>();
                    // loop through Room Type lines, 
                    while (!string.IsNullOrEmpty(roomTypesLine))
                    {
                        string[] roomTypesCells = roomTypesLine.Split('|');
                        if (!roomTypeDescriptions.ContainsKey(roomTypesCells[1]))
                        {
                            roomTypeDescriptions.Add(roomTypesCells[1], HttpUtility.HtmlEncode(roomTypesCells[2]));
                        }
                        
                        roomTypesLine = roomTypes.ReadLine()!;
                    }

                    return roomTypeDescriptions;
                }

                var roomTypeDescriptions = cache.GetOrCreate("RoomTypeDescriptions", cacheBuilder, 9999);

                return roomTypeDescriptions.ContainsKey(hotelCode + "-" + roomTypeCode)
                    ? roomTypeDescriptions[hotelCode + "-" + roomTypeCode]
                    : "";
            }

            public PropertyGroupings GetPropertyGroupings(IMemoryCache cache)
            {
                return new PropertyGroupings
                {
                    Properties = GetDistinctProperties().Select(propertyCode => new Property
                    {
                        PropertyCode = propertyCode,
                        RoomTypes = GetPropertyRoomTypes(propertyCode)
                            .Select(roomType => new RoomType 
                            {
                                RoomTypeCode = roomType.Key,
                                RoomTypeDescription = new RoomTypeDescription 
                                { 
                                    Text = !string.IsNullOrEmpty(roomType.Value)
                                    ? roomType.Value
                                    : GetRoomTypeDescription(propertyCode, roomType.Key, cache)
                                },
                                PropertyRoomBookings = GetPropertyRoomBooking(propertyCode, roomType.Key)
                                    .Select(roomBooking => new PropertyRoomBooking
                                    {
                                        ID = roomBooking
                                    }).ToArray(),
                            }).ToArray()
                    }).ToArray()
                };
            }
        }

        public class Grouping
        {
            public string PropertyCode;
            public string RoomTypeCode;
            public int PropertyRoomBooking;
            public string RoomTypeDescription;
        }
    }
}
