namespace ThirdParty.CSSuppliers.iVectorChannelManager
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using Intuitive.Helpers.Serialization;
    using Models;
    using ThirdParty.Constants;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Results.Models;

    public class ChannelManagerSearch : IThirdPartySearch, ISingleSource
    {
        private readonly IChannelManagerSettings _settings;

        private readonly ISerializer _serializer;

        public ChannelManagerSearch(IChannelManagerSettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        public string Source => ThirdParties.CHANNELMANAGER;

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = new List<Request>();

            var searchRequest = BuildSearchRequests(searchDetails, resortSplits);

            var request = new Request
            {
                EndPoint = _settings.GenericURL(searchDetails),
                Method = RequestMethod.POST,
                ContentType = ContentTypes.Application_x_www_form_urlencoded,
                ExtraInfo = searchDetails,
            };

            request.SetRequest(searchRequest);

            requests.Add(request);

            return Task.FromResult(requests);
        }

        public XmlDocument BuildSearchRequests(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var propertyReferenceIDs = resortSplits
                .SelectMany(x => x.Hotels)
                .Select(x => x.TPKey.ToSafeInt())
                .ToList();

            var searchRequest = new SearchRequest(Helper.GetLoginDetails(searchDetails, _settings))
            {
                PropertyReferenceIDs = propertyReferenceIDs,
                CheckInDate = searchDetails.ArrivalDate.ToString("yyyy-MM-dd"),
                CheckOutDate = searchDetails.DepartureDate.ToString("yyyy-MM-dd"),
                BrandID = _settings.BrandCode(searchDetails),
            };

            foreach (var room in searchDetails.RoomDetails)
            {
                var roomRequest = new SearchRequest.Room
                {
                    Seq = room.PropertyRoomBookingID,
                    Adults = room.Adults,
                    Infants = room.Infants,
                    Children = room.Children
                };

                if (room.Children > 0)
                {
                    var childAges = new List<int>();

                    foreach (var age in room.ChildAges)
                    {
                        childAges.Add(age);
                    }

                    roomRequest.ChildAgeCSV = string.Join(",", childAges);
                }

                searchRequest.Rooms.Add(roomRequest);
            }

            return _serializer.Serialize(searchRequest);
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();

            var allResponses =
                from request in requests
                where request.Success
                select _serializer.DeSerialize<SearchResponse>(request.ResponseString);

            transformedResults.TransformedResults.AddRange(allResponses.SelectMany(r => GetResultFromResponse(r)));

            return transformedResults;
        }

        public List<TransformedResult> GetResultFromResponse(SearchResponse response)
        {
            var transformedResult = new List<TransformedResult>();

            foreach (var propertyResult in response.Properties)
            {
                foreach (var roomType in propertyResult.Rooms)
                {
                    var adjustments = new List<TransformedResultAdjustment>();
                    foreach (var adjustment in roomType.Adjustments)
                    {
                        var transformedAdjustment = new TransformedResultAdjustment(
                            adjustment.AdjustmentType.ToSafeEnum<SDK.V2.PropertySearch.AdjustmentType>()
                                ?? SDK.V2.PropertySearch.AdjustmentType.Supplement,
                            adjustment.AdjustmentName,
                            string.Empty,
                            adjustment.AdjustmentAmount);
                        adjustments.Add(transformedAdjustment);
                    }

                    transformedResult.Add(new TransformedResult
                    {
                        TPKey = propertyResult.PropertyReferenceID.ToString(),
                        CurrencyCode = propertyResult.CurrencyID.ToString(),
                        PropertyRoomBookingID = roomType.Seq,
                        RoomType = roomType.RoomType,
                        MealBasisCode = roomType.MealBasisID.ToString(),
                        Amount = roomType.NetCost,
                        TPReference = roomType.RoomBookingToken,
                        CommissionPercentage = roomType.CommissionPercentage,
                        NonRefundableRates = roomType.NonRefundable,
                        RateCode = roomType.RateCode,
                        Adjustments = adjustments
                    });
                }
            }

            return transformedResult;

        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return false;
        }

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }
    }
}