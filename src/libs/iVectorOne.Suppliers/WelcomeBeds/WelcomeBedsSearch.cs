namespace iVectorOne.Suppliers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.Models.WelcomeBeds;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;

    public class WelcomeBedsSearch : IThirdPartySearch, ISingleSource
    {
        #region Constructor

        public readonly IWelcomeBedsSettings _settings;
        public readonly ISerializer _serializer;

        public WelcomeBedsSearch(IWelcomeBedsSettings settings, ISerializer serializer)
        {
            _settings = settings;
            _serializer = serializer;
        }

        #endregion

        #region Properties

        public string Source => ThirdParties.WELCOMEBEDS;

        #endregion

        #region Build Search Request

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = new List<Request>();

            //'Create a list of the destinations to search splitting things into region or resort depending on a cap setting to minimise the amount of searches for large areas
            var searchDestinations = GenerateDestinations(_settings.ResortSearchCap(searchDetails), resortSplits);

            //'Search details for single hotel search
            var hotelCode = "";
            if (resortSplits.Count == 1)
            {
                if (resortSplits.First().Hotels.Count == 1) hotelCode = resortSplits.First().Hotels.First().TPKey;
            }

            //'Sends a search off for each resort split
            foreach (var searchDestination in searchDestinations)
            {
                var availRequest = new OtaHotelAvailRq { Version = _settings.Version(searchDetails) };
                var availRequestSegment = new AvailRequestSegment();
                availRequest.AvailRequestSegmets = new List<AvailRequestSegment> { availRequestSegment };

                //'Dates
                availRequestSegment.StayDateRange = new StayDateRange
                {
                    Start = searchDetails.ArrivalDate.ToString(Constant.DateFormat),
                    End = searchDetails.DepartureDate.ToString(Constant.DateFormat)
                };

                //'Add Guests to the Room
                availRequestSegment.RoomStayCandidates = searchDetails.RoomDetails.Select(roomInfo =>
                {
                    var guestCounts = new List<GuestCount>();
                    guestCounts.AddRange(Enumerable.Range(0, roomInfo.Adults).Select(_ => new GuestCount { Count = 1, Age = 30 }));
                    guestCounts.AddRange(roomInfo.ChildAges.Select(childAge => new GuestCount { Count = 1, Age = childAge }));
                    guestCounts.AddRange(Enumerable.Range(0, roomInfo.Infants).Select(_ => new GuestCount { Count = 1, Age = 1 }));
                    return new RoomStayCandidate { GuestCounts = guestCounts };
                }).ToList();

                //'Single property search if needs be
                if (!string.IsNullOrEmpty(hotelCode))
                {
                    availRequestSegment.HotelSearchCriteria = new List<Criterion>
                    {
                        new Criterion
                        {
                            HotelRef = new HotelRef { HotelCode = hotelCode }
                        }
                    };
                }


                //'Split up the Gepgraphy into its bits
                var destinationCodes = searchDestination.Split('|');
                var providerAreas = new List<Area> {
                            new Area {  TypeCode = "Country", AreaCode = destinationCodes[0] },
                            new Area {  TypeCode = "Province", AreaCode = destinationCodes[1] },
                };
                //'If we are doing a resort search then add this too
                if (destinationCodes.Length == 3)
                    providerAreas.Add(new Area { TypeCode = "Town", AreaCode = destinationCodes[2] });



                availRequestSegment.TpaExtensions = new TpaExtensions
                {
                    Providers = new List<Provider> { new Provider
                    {
                        Name = "GSI",
                        //'Credentials
                        Credentials = BuildCredentials(searchDetails, _settings),
                        //'Geography
                        ProviderAreas = providerAreas,
                    }},
                    ProviderTokens = new List<Token> {
                        new Token { TokenName="ResponseMode", TokenCode = "4" }
                    }
                };

                var xmlDoc = Envelope<OtaHotelAvailRq>.Serialize(availRequest, _serializer);

                //'set a unique code. if the is one request we only need the source name
                string uniqueCode = Source;
                if (searchDestinations.Count() > 1) uniqueCode = $"{Source}_{searchDestination}";

                var request = new Request
                {
                    EndPoint = _settings.GenericURL(searchDetails),
                    Method = RequestMethod.POST,
                    UseGZip = true,
                    SoapAction = "HotelAvail"
                };
                request.SetRequest(xmlDoc);

                requests.Add(request);
            }

            return Task.FromResult(requests);
        }

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return searchDetails.Rooms > 3;
        }

        public List<string> GenerateDestinations(int resortSearchCap, List<ResortSplit> resortSplits)
        {
            // To reduce the number of searches requests that we send,
            //   we will put a cap on the number of resorts that are in the same region, and if it is more than the cap
            //      then we will do a region search instead of a resort and reduce the amount of requests

            var searchDestinations = resortSplits.GroupBy(rs => string.Join("|", rs.ResortCode.Split('|').Take(2)))
                .Aggregate(new List<string>(), (list, gr) =>
                {
                    if (gr.Count() > resortSearchCap)
                    {
                        // 'Add just country and region code to the list
                        list.Add(gr.Key);
                    }
                    else
                    {
                        list.AddRange(gr.Select(g => g.ResortCode).Distinct());
                    }
                    return list;
                });

            return searchDestinations;
        }

        public static List<Credential> BuildCredentials(IThirdPartyAttributeSearch searchDetails, IWelcomeBedsSettings settings)
        {
            return new List<Credential> {
                            new Credential { CredentialCode = settings.AccountCode(searchDetails), CredentialName = "AccountCode" },
                            new Credential { CredentialCode = settings.Password(searchDetails), CredentialName = "Password" },
                            new Credential { CredentialCode = settings.System(searchDetails), CredentialName = "System" },
                            new Credential { CredentialCode = settings.SalesChannel(searchDetails), CredentialName = "SalesChannel" },
                            new Credential { CredentialCode = settings.LanguageCode(searchDetails), CredentialName = "Language" },
                            new Credential { CredentialCode = settings.ConnectionString(searchDetails), CredentialName = "ConnectionString" },
                        };
        }
        #endregion

        #region Transform Response

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();
            var allResponses = requests.Where(rq => rq.Success)
                .Select(rq => Envelope<OtaHotelAvailRs>.DeSerialize(rq.ResponseXML, _serializer)).ToList();

            transformedResults.TransformedResults.AddRange(allResponses.SelectMany(r => GetResultFromResponse(r, searchDetails)));
            
            return transformedResults;
        }

        private string RoomComparableCode(int adults, int child12OrOver, int childUnder12, int infants)
        {
            return $"{adults}|{child12OrOver}|{childUnder12}|{infants}";
        }

        public List<TransformedResult> GetResultFromResponse(OtaHotelAvailRs response, SearchDetails searchDetails)
        {
            var roomDatails = searchDetails.RoomDetails
                .Select((rd, i) => new
                {
                    PropertyRoomBookingId = i + 1,
                    RoomComparable = RoomComparableCode(rd.Adults, rd.Child12OrOver, rd.Children - rd.Child12OrOver, rd.Infants)
                });

            var transformedResults = response.RoomStays.SelectMany(roomStay =>
            {
                var tpk = roomStay.TpaExtensions.HotelInfo.Id;
                return roomStay.RoomRates
                    .Where(rr => string.Equals(rr.AvailabilityStatus, Constant.AvailableForSale))
                    .Select(roomRate =>
                {
                    var roomTypeCode = roomRate.RoomTypeCode;
                    var mealBasisCode = roomStay.RatePlans.First(rp => rp.RatePlanCode == roomRate.RatePlanCode)
                                                        .MealsIncluded.MealPlanCodes.Split('-')[0];
                    var currencyCode = roomRate.Rates.First().Total.CurrencyCode;
                    var adults = roomRate.GuestCounts.Where(gc => gc.Age == 30).Count();
                    var childrenUnder12 = roomRate.GuestCounts.Where(gc => gc.Age < 12 && gc.Age > 1).Count();
                    var childrenOver12 = roomRate.GuestCounts.Where(gc => gc.Age < 30 && gc.Age > 11).Count();
                    var infants = roomRate.GuestCounts.Where(gc => gc.Age == 1).Count();

                    var roomComparable = RoomComparableCode(adults, childrenOver12, childrenUnder12, infants);

                    var tpReference = roomRate.Rates.First().TpaExtensions.RoomToken.Token;

                    return new TransformedResult
                    {
                        TPKey = tpk.Id,
                        CurrencyCode = currencyCode,
                        RoomType = roomStay.RoomTypes.First(rt => rt.RoomTypeCode == roomRate.RoomTypeCode).RoomDescription.Name,
                        RoomTypeCode = roomTypeCode,
                        MealBasisCode = mealBasisCode,
                        PropertyRoomBookingID = roomDatails.FirstOrDefault(rd => string.Equals(rd.RoomComparable, roomComparable)).PropertyRoomBookingId,
                        Amount = roomRate.Rates.First().Total.AmountAfterTax.ToSafeDecimal(),
                        NonRefundableRates = string.Equals("true", roomRate.Rates.First().CancelPolicies?.FirstOrDefault()?.NonRefundable.ToLower() ?? string.Empty),
                        TPReference = $"{tpReference}|{roomTypeCode}|{mealBasisCode}|{roomRate.PromotionCode}|{currencyCode}"
                    };
                });
            }).ToList();

            return transformedResults;
        }

        #endregion
    }
}