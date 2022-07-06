namespace ThirdParty.CSSuppliers.iVectorConnect
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CSSuppliers.iVectorConnect.Models;
    using CSSuppliers.iVectorConnect.Models.Common;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using booking = ThirdParty.Models.Property.Booking;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Results.Models;

    public class iVectorConnectSearch : IThirdPartySearch, IMultiSource
    {
        private readonly IiVectorConnectSettings _settings;
        private readonly ISerializer _serializer;

        public iVectorConnectSearch(IiVectorConnectSettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        public List<string> Sources => Helper.iVectorConnectSources;

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            string source = resortSplits.First().ThirdPartySupplier;
            var requests = new List<Request>();
            string url = _settings.SearchURL(searchDetails, source);

            var searchRequest = BuildSearchRequest(searchDetails, resortSplits, source);

            var request = new Request
            {
                EndPoint = url,
                Method = RequestMethod.POST,
                ContentType = ContentTypes.Application_xml,
                ExtraInfo = searchDetails,
            };

            request.SetRequest(_serializer.Serialize(searchRequest));

            requests.Add(request);

            return Task.FromResult(requests);
        }

        public PropertySearchRequest BuildSearchRequest(SearchDetails searchDetails, List<ResortSplit> resortSplits, string source)
        {
            var searchRequest = new PropertySearchRequest()
            {
                LoginDetails = Helper.GetLoginDetails(searchDetails, _settings, source),
            };

            searchRequest.PropertyReferenceIDs = resortSplits
                .SelectMany(r => r.Hotels)
                .Select(h => h.TPKey.ToSafeInt())
                .ToList();

            searchRequest.ArrivalDate = searchDetails.ArrivalDate.ToString("yyyy-MM-dd");
            searchRequest.Duration = searchDetails.Duration;

            foreach (var room in searchDetails.RoomDetails)
            {
                var guestConfiguration = new GuestConfiguration
                {
                    Adults = room.Adults,
                    Infants = room.Infants,
                    Children = room.Children
                };

                if (room.Children > 0)
                {
                    int[] childAges = new int[room.ChildAges.Count];

                    for (int i = 0; i < room.ChildAges.Count; i++)
                    {
                        childAges[i] = room.ChildAges[i];
                    }

                    guestConfiguration.ChildAges = childAges;
                }

                searchRequest.RoomRequests.Add(new RoomRequest { GuestConfiguration = guestConfiguration });
            }

            searchRequest.MealBasisID = searchDetails.MealBasisID;

            if (!string.IsNullOrEmpty(searchDetails.StarRating))
            {
                searchRequest.MinStarRating = searchDetails.StarRating;
            }

            if (searchDetails.ProductAttributeIDs.Count > 0)
            {
                searchRequest.ProductAttributes = searchDetails.ProductAttributeIDs;
            }

            return searchRequest;
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();
            string source = resortSplits.First().ThirdPartySupplier;

            var allResponses =
                from request in requests
                where request.Success
                select _serializer.DeSerialize<PropertySearchResponse>(request.ResponseXML);

            transformedResults.TransformedResults
                .AddRange(allResponses.SelectMany(r => GetResultFromResponse(r, searchDetails, source)));

            return transformedResults;
        }

        public List<TransformedResult> GetResultFromResponse(PropertySearchResponse response, SearchDetails searchDetails, string source)
        {
            var transformedResult = new List<TransformedResult>();

            foreach (var propertyResult in response.PropertyResults)
            {
                foreach (var roomType in propertyResult.RoomTypes)
                {
                    transformedResult.Add(new TransformedResult
                    {
                        TPKey = propertyResult.PropertyReferenceID.ToString(),
                        CurrencyCode = roomType.SupplierDetails.CurrencyID.ToString(),
                        PropertyRoomBookingID = roomType.Seq,
                        RoomType = roomType.RoomTypeProperty,
                        MealBasisCode = roomType.MealBasisID.ToString(),
                        Amount = roomType.Total,
                        TPReference = $"{propertyResult.BookingToken}|{roomType.RoomBookingToken}",
                        SpecialOffer = roomType.SpecialOffer,
                        TPRateCode = roomType.SupplierDetails.RateCode,
                        NonRefundableRates = roomType.NonRefundable,
                        Discount = roomType.Adjustments.Where(a => a.AdjustmentType == "Offer").Sum(a => -a.Total),
                        Cancellations = roomType.SupplierCancellations
                            .Select(c => new booking.Cancellation(c.StartDate, c.EndDate, c.Amount))
                            .ToList(),
                    });
                }
            }

            return transformedResult;

        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source) => false;

        public bool ResponseHasExceptions(Request request) => false;
    }
}