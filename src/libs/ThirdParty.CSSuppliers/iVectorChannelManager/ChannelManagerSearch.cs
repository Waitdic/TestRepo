namespace ThirdParty.CSSuppliers.iVectorChannelManager
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using Models;
    using ThirdParty.Constants;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Results.Models;

    public class ChannelManagerSearch : IThirdPartySearch, ISingleSource
    {
        private readonly IChannelManagerSettings _settings;

        private readonly Serializer serializer = new();

        public ChannelManagerSearch(IChannelManagerSettings settings)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
        }

        public string Source => ThirdParties.CHANNELMANAGER;

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = new List<Request>();

            var searchRequest = BuildSearchRequests(searchDetails, resortSplits);

            var request = new Request
            {
                EndPoint = _settings.URL(searchDetails),
                Method = eRequestMethod.POST,
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
                CheckInDate = searchDetails.PropertyArrivalDate.ToString("yyyy-MM-dd"),
                CheckOutDate = searchDetails.PropertyArrivalDate.AddDays(searchDetails.Duration).ToString("yyyy-MM-dd"),
                BrandID = _settings.BrandID(searchDetails),
            };

            var rooms = new List<SearchRequest.Room>();

            var seq = 1;

            foreach (var room in searchDetails.RoomDetails)
            {
                var roomRequest = new SearchRequest.Room
                {
                    Seq = seq,
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

                    roomRequest.ChildAgeCSV = string.Join<int>(",", childAges);
                }

                rooms.Add(roomRequest);

                seq++;
            }

            searchRequest.Rooms = rooms;

            return serializer.Serialize(searchRequest);

        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();

            var allResponses =
                from request in requests
                where request.Success
                select serializer.DeSerialize<SearchResponse>(request.ResponseString);

            transformedResults.TransformedResults.AddRange(allResponses.SelectMany(r => GetResultFromResponse(r, searchDetails)));

            return transformedResults;
        }

        public List<TransformedResult> GetResultFromResponse(SearchResponse response, SearchDetails searchDetails)
        {
            var transformedResult = new List<TransformedResult>();

            foreach (var propertyResult in response.Properties)
            {
                foreach (var roomType in propertyResult.Rooms)
                {
                    var adjustments = new List<TransformedResultAdjustment>();
                    foreach (var adjustment in roomType.Adjustments)
                    {
                        var transformedAdjustment = new TransformedResultAdjustment
                        {
                            AdjustmentID = adjustment.AdjustmentID,
                            AdjustmentType = adjustment.AdjustmentType,
                            AdjustmentName = adjustment.AdjustmentName,
                            AdjustmentAmount = adjustment.AdjustmentAmount,
                            PayLocal = adjustment.PayLocal
                        };
                        adjustments.Add(transformedAdjustment);
                    }

                    transformedResult.Add(new TransformedResult
                    {
                        TPKey = propertyResult.PropertyReferenceID.ToSafeString(),
                        CurrencyID = propertyResult.CurrencyID,
                        PropertyRoomBookingID = roomType.Seq,
                        RoomType = roomType.RoomType,
                        MealBasisID = roomType.MealBasisID,
                        NetPrice = roomType.NetCost.ToSafeString(),
                        TPReference = roomType.RoomBookingToken,
                        CommissionPercentage = roomType.CommissionPercentage,
                        NonRefundableRates = roomType.NonRefundable,
                        TPRateCode = roomType.RateCode,
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