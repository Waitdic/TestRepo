namespace iVectorOne.Suppliers.RMI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using iVector.Search.Property;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.RMI.Models;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;

    public class RMISearch : IThirdPartySearch, ISingleSource
    {
        #region Properties

        private readonly IRMISettings _settings;
        private readonly ISerializer _serializer;

        public string Source => ThirdParties.RMI;

        #endregion

        #region Constructors

        public RMISearch(IRMISettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        #endregion

        #region Search restrictions

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return false;
        }

        #endregion

        #region Search functions

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = new List<Request>();
            var hotels = resortSplits.SelectMany(resort => resort.Hotels).ToList();
            if (hotels.Any()) 
            {
                var requestedMealBasisIds = _settings.RequestedMealBases(searchDetails)
                                                                        .Split(',')
                                                                        .Select(mb => mb.ToSafeInt())
                                                                        .Where(mb => mb != 0)
                                                                        .Take(10).ToList();

                if (!requestedMealBasisIds.Any())
                {
                    requestedMealBasisIds.Add(0);
                }

                requests = requestedMealBasisIds.Select(mealBasisId =>
                {
                    string request = BuildSearchXml(searchDetails, hotels, mealBasisId);
                    return BuildRequest(searchDetails, request);
                }).ToList();
            }
            return Task.FromResult(requests);
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();

            var searchResponses = requests.Select(request => _serializer.DeSerialize<SearchResponse>(request.ResponseXML)).ToList();
            var xmls = searchResponses.SelectMany(searchResponse =>
            {
                return searchResponse.PropertyResults.SelectMany(propertyResult =>
                {
                    return propertyResult.RoomTypes
                        .Where(rt => string.Equals(rt.OnRequest.ToLower(), "false"))
                        .Select(roomType =>
                    {
                        return new TransformedResult
                        {
                            TPKey = propertyResult.PropertyId,
                            CurrencyCode = "USD",
                            TPReference = roomType.RoomId,
                            RoomType = roomType.Name,
                            RoomTypeCode = roomType.RoomId,
                            MealBasisCode = roomType.MealBasisId,
                            Amount = roomType.Total,
                            PropertyRoomBookingID = roomType.RoomsAppliesTo.RoomRequest,
                            SpecialOffer = string.Join(",", roomType.SpecialOffers.Select(offer => offer.Name)),
                            Discount = roomType.SpecialOffers.Sum(offer => offer.Total.ToSafeDecimal()),
                            MasterID = propertyResult.PropertyId.ToSafeInt(),
                            NonRefundableRates = IsNonRefundable(roomType)
                        };
                    });
                });
            }).ToList();
            transformedResults.TransformedResults.AddRange(xmls);            

            return transformedResults;
        }

        private bool IsNonRefundable(RoomType roomType)
        {
            var fullFeeForNow = roomType.CancellationPolicies.Any(canx =>
            {
                DateTime startDate = canx.CancelBy.ToSafeDate();
                DateTime now = DateTime.Now;
                decimal fee = canx.Penalty.ToSafeDecimal();
                return startDate < now && fee >= roomType.Total;
            });

            return fullFeeForNow;
        }

        public string BuildSearchXml(SearchDetails searchDetails, List<Hotel> hotels, int mealBasisId)
        {
            var searchRequest = new SearchRequest
            {
                LoginDetails =
                {
                    Login = _settings.Login(searchDetails),
                    Password = _settings.Password(searchDetails),
                    Version = _settings.Version(searchDetails)
                },
                SearchDetails =
                {
                    ArrivalDate = searchDetails.ArrivalDate.ToString(Constant.DateFormat),
                    Duration = searchDetails.Duration,
                    PropertyID = (hotels.Count == 1)?hotels[0].TPKey: "",
                    Properties = (hotels.Count > 1)?hotels.Select(hotel => hotel.TPKey).ToList():new List<string>(),
                    MealBasisId = mealBasisId.ToString(),
                    MinStarRating = 0,
                    MinimumPrice = 0,
                    MaximumPrice = 0,
                    RoomRequests = searchDetails.RoomDetails.Select(oRoomDetail => new RoomRequest
                    {
                        Adults = oRoomDetail.Adults,
                        Children = oRoomDetail.Children,
                        Infants = oRoomDetail.Infants,
                        ChildAges = oRoomDetail.ChildAges.Select(iAge => new ChildAge
                        {
                            Age = iAge
                        }).ToList()
                    }).ToList()
                },
            };

            var xmlDoc = _serializer.Serialize(searchRequest);
            return xmlDoc.OuterXml;
        }

        public Request BuildRequest(SearchDetails searchDetails, string request)
        {
            var webRequest = new Request
            {
                EndPoint = _settings.URL(searchDetails),
                Method = RequestMethod.POST,
                AuthenticationMode = AuthenticationMode.Basic,
                UseGZip = _settings.UseGZip(searchDetails)
            };

            webRequest.SetRequest(request);

            return webRequest;
        }

        #endregion

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }
    }
}