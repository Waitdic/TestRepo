namespace iVectorOne.Suppliers.MTS
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using iVectorOne.Suppliers.MTS.Models.Search;
    using iVectorOne.Suppliers.MTS.Models.Common;
    using iVectorOne.Models.Property;

    public class MTSSearch : IThirdPartySearch, ISingleSource
    {
        #region Constructor

        private readonly IMTSSettings _settings;
        private readonly ISerializer _serializer;

        public MTSSearch(IMTSSettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        #endregion

        #region Properties

        public string Source => ThirdParties.MTS;

        #endregion

        #region SearchRestrictions

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return false;
        }

        #endregion

        #region SearchFunctions

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = new List<Request>();
            var regions = new Dictionary<string, string>();

            var overrideCountriesList = new List<string>();
            if (overrideCountriesList.Count == 0)
            {
                string overrideCountries = _settings.OverrideCountries(searchDetails);
                if (!string.IsNullOrWhiteSpace(overrideCountries))
                {
                    // split the string and add each one to _overrideCountries
                    foreach (string country in overrideCountries.Split('|'))
                    {
                        overrideCountriesList.Add(country);
                    }
                }
            }

            if (resortSplits.Count == 1)
            {
                // how many hotels? if >1, search by resort, if =1 search by hotelcode
                if (resortSplits[0].Hotels.Count == 1)
                {
                    string country = resortSplits[0].ResortCode.Split('|')[0];
                    string hotelCode = resortSplits[0].Hotels[0].TPKey;
                    string countryAndHotelCode = country + "|" + hotelCode;

                    regions.Add(countryAndHotelCode, "CountryAndHotelCode");
                }

                if (resortSplits[0].Hotels.Count > 1)
                {
                    string resortPath = resortSplits[0].ResortCode;

                    // save resort if new
                    if (!regions.ContainsKey(resortPath))
                    {
                        regions.Add(resortPath, "Resort");
                    }
                }
            }

            if (resortSplits.Count > 1)
            {
                // select region and save; for each new different region, save that region as well
                foreach (var resortSplit in resortSplits)
                {
                    // select region
                    string country = resortSplit.ResortCode.Split('|')[0];
                    string region = resortSplit.ResortCode.Split('|')[1];
                    string regionPath = country + "|" + region;

                    // save region if new
                    if (!regions.ContainsKey(regionPath))
                    {
                        regions.Add(regionPath, "Region");
                    }
                }
            }

            // need to send off a request for each resort and store them in an array
            // build request
            foreach (var search in regions)
            {
                // get the third party resorts
                // once get IPs confirmed, ie not now
                var searchKey = search.Key.Split('|');

                var useOverrideId = overrideCountriesList.Contains(search.Key.Split('|')[0]);

                HotelSearchCriteria hotelSearchCriteria;
                if (search.Value == "CountryAndHotelCode")
                {
                    hotelSearchCriteria = new HotelSearchCriteria
                    {
                        Criterion = new Criterion
                        {
                            ExactMatch = true,
                            HotelRef = new HotelRef { HotelCode = search.Key.Split('|')[1] }
                        }
                    };
                }
                else
                {
                    var refPoints = new List<RefPoint>
                    {
                        new() { CodeContext = "Country", RefPointValue = search.Key.Split('|')[0] },
                        new() { CodeContext = "Region", RefPointValue = search.Key.Split('|')[1] }
                    };

                    // Check if is a resort-level search
                    if (search.Value == "Resort")
                    {
                        refPoints.Add(new RefPoint { CodeContext = "Resort", RefPointValue = search.Key.Split('|')[2] });
                    }

                    hotelSearchCriteria = new HotelSearchCriteria
                    {
                        Criterion = new Criterion { RefPoint = refPoints.ToArray() }
                    };
                }
                
                // build the request
                var request = new MTSSearchRequest
                {
                    Version = "0.1",
                    POS = { Source = new[]
                    {
                        new Source
                        {
                            RequestorID =
                            {
                                Instance = _settings.Instance(searchDetails),
                                ID_Context =  _settings.ID_Context(searchDetails),
                                ID = useOverrideId ? _settings.OverRideID(searchDetails) : _settings.User(searchDetails),
                                Type =  _settings.Type(searchDetails)
                            },
                            BookingChannel = new BookingChannel { Type = 2 }
                        },
                        new Source
                        {
                            RequestorID =
                            {
                                ID = _settings.AuthenticationID(searchDetails),
                                Type = _settings.AuthenticationType(searchDetails),
                                MessagePassword = _settings.Password(searchDetails)
                            }
                        }
                    }},
                    AvailRequestSegments = new []
                    {
                        new AvailRequestSegment
                        {
                            InfoSource = "1*2*4*5*",
                            StayDateRange =
                            {
                                End = searchDetails.DepartureDate.ToString("yyyy-MM-dd"),
                                Start = searchDetails.ArrivalDate.ToString("yyyy-MM-dd")
                            },
                            // loop through the rooms
                            RoomStayCandidates = searchDetails.RoomDetails.Select((roomBooking, roomCount) =>
                            {
                                // Adults
                                var guestCounts = new List<GuestCount>
                                {
                                    new() { AgeQualifyingCode = "10", Count = roomBooking.Adults }
                                };

                                // Children
                                guestCounts.AddRange(roomBooking.ChildAges.Select(childAge => new GuestCount
                                {
                                    AgeQualifyingCode = "8",
                                    Age = childAge,
                                    Count = 1
                                }));

                                // Infants
                                if (roomBooking.Infants > 0)
                                {
                                    guestCounts.Add(new GuestCount
                                    {
                                        AgeQualifyingCode = "7",
                                        Age = 1,
                                        Count = roomBooking.Infants
                                    });
                                }

                                return new RoomStayCandidate
                                {
                                    RPH = roomCount + 1,
                                    GuestCounts = guestCounts.ToArray()
                                };
                            }).ToArray(),
                            HotelSearchCriteria = hotelSearchCriteria
                        }
                    }
                };

                var webRequest = new Request
                {
                    EndPoint = _settings.GenericURL(searchDetails),
                    Method = RequestMethod.POST,
                    ContentType = ContentTypes.Application_json
                };
                webRequest.SetRequest(_serializer.Serialize(request));
                requests.Add(webRequest);
            }

            return Task.FromResult(requests);
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();
            var allResponses = new List<MTSSearchResponse>();

            foreach (var request in requests)
            {
                var response = new MTSSearchResponse();
                bool success = request.Success;

                if (success)
                {
                    response = _serializer.DeSerialize<MTSSearchResponse>(request.ResponseString);

                    allResponses.Add(response);
                }
            }

            transformedResults.TransformedResults
                .AddRange(allResponses.Where(o => o.Hotels.Count() > 0)
                .SelectMany(r => GetResultFromResponse(searchDetails, r)));

            return transformedResults;
        }


        private List<TransformedResult> GetResultFromResponse(SearchDetails searchDetails, MTSSearchResponse response)
        {
            var transformedResults = new List<TransformedResult>();

            foreach (var hotel in response.Hotels)
            {
                foreach (var room in response.Rooms)
                {
                    if (hotel.Info.HotelCode == room.Info.HotelCode)
                    {
                        string ratePlan = room.RatePlans.Count > 0 ? room.RatePlans.First().RatePlanCode : string.Empty;
                        foreach (var area in response.Areas)
                        {
                            if (area.AreaID == hotel.Info.AreaID)
                            {
                                string country = area.Descriptions.Where(x => x.Name == "Country").FirstOrDefault().Text;

                                foreach (var roomType in room.RoomTypes)
                                {
                                    foreach (var roomRate in room.RoomRates)
                                    {
                                        var amount = roomRate.Rates.FirstOrDefault().Total.AmountAfterTax;

                                        if (roomRate.NumberOfUnits < searchDetails.Rooms || amount == 0)
                                        {
                                            continue;
                                        }

                                        transformedResults.Add(new TransformedResult()
                                        {
                                            MasterID = 0,
                                            TPKey = hotel.Info.HotelCode,
                                            CurrencyCode = roomRate.Rates.FirstOrDefault().Total.CurrencyCode,
                                            PropertyRoomBookingID = room.ID,
                                            NonRefundableRates = roomType.Code.Substring(10) == "N",
                                            RoomType = roomType.RoomDescription.Text,
                                            MealBasisCode = roomRate.Features.FirstOrDefault().Descriptions.FirstOrDefault().Text,
                                            Amount = amount,
                                            TPReference = $"{roomType.Code}|{roomRate.Features.FirstOrDefault().Descriptions.FirstOrDefault().Text}|{country}|{ratePlan}",
                                            RoomTypeCode = roomRate.RoomTypeCode
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return transformedResults;
        }

        #endregion

        #region ResponseHasExceptions

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        #endregion
    }
}