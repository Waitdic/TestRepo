namespace ThirdParty.CSSuppliers.iVectorConnect
{
    using System.Collections.Generic;
    using System.Linq;
    using CSSuppliers.iVectorConnect.Models;
    using CSSuppliers.iVectorConnect.Models.Common;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;

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

        public List<Request> BuildSearchRequests(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            //1. set up some start values
            string source = resortSplits.First().ThirdPartySupplier;
            var requests = new List<Request>();
            string url = _settings.URL(searchDetails, source);

            //2. build the xml
            var searchRequest = BuildSearchXml(searchDetails, resortSplits, source);

            //3. set up the request 
            var request = new Request
            {
                EndPoint = url,
                Method = eRequestMethod.POST,
                ContentType = ContentTypes.Application_x_www_form_urlencoded,
                ExtraInfo = searchDetails,
            };

            request.SetRequest(_serializer.Serialize(searchRequest));

            requests.Add(request);

            return requests;
        }

        public PropertySearchRequest BuildSearchXml(SearchDetails searchDetails, List<ResortSplit> resortSplits, string source)
        {
            bool validSearch = false;

            //Get login details
            var searchRequest = new PropertySearchRequest(Helper.GetLoginDetails(searchDetails, _settings, source));

            //Single property
            if (resortSplits.Count == 1 && resortSplits[0].Hotels.Count == 1)
            {
                validSearch = true;
                int propertyId = resortSplits[0].Hotels[0].TPKey.ToSafeInt();

                searchRequest.PropertyReferenceID = propertyId;
            }

            //Multiple properties
            if (searchDetails.PropertyReferenceIDs.Count > 0)
            {
                validSearch = true;
                int propertyReferenceIdIndex = 0;
                int[] propertyReferenceIDs = new int[GetPropertyReferenceIDsCount(resortSplits)];

                foreach (var resort in resortSplits)
                {
                    foreach (var hotel in resort.Hotels)
                    {
                        propertyReferenceIDs[propertyReferenceIdIndex] = hotel.TPKey.ToSafeInt();
                        propertyReferenceIdIndex++;
                    }
                }

                searchRequest.PropertyReferenceIDs = propertyReferenceIDs;
            }

            //Geography Search
            //Region
            if (searchDetails.GeographyLevel2ID != 0)
            {
                searchRequest.RegionID = resortSplits[0].ResortCode.Split('_')[1];
            }

            //Resort Search
            if (!validSearch)
            {
                int[] resorts = new int[resortSplits.Count];

                for (int i = 0; i < resortSplits.Count; i++)
                {
                    resorts[i] = resortSplits[i].ResortCode.Split('_')[0].ToSafeInt();
                }
                validSearch = true;

                searchRequest.Resorts = resorts;
            }

            searchRequest.ArrivalDate = searchDetails.PropertyArrivalDate.ToString("yyyy-MM-dd");
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

            //Room Details
            searchRequest.MealBasisID = searchDetails.MealBasisID;

            if (!string.IsNullOrEmpty(searchDetails.StarRating))
            {
                searchRequest.MinStarRating = searchDetails.StarRating;
            }

            //Product Attributes
            if (searchDetails.ProductAttributeIDs.Count > 0)
            {
                int[] productAttributes = new int[searchDetails.ProductAttributeIDs.Count];
                for (int i = 0; i < searchDetails.ProductAttributeIDs.Count; i++)
                {
                    productAttributes[i] = searchDetails.ProductAttributeIDs[i];
                }
                searchRequest.ProductAttributes = productAttributes;
            }

            return searchRequest;
        }

        private static int GetPropertyReferenceIDsCount(IEnumerable<ResortSplit> resortSplits)
        {
            return resortSplits.Sum(resortSplit => resortSplit.Hotels.Count);
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
            int currency = _settings.SellingCurrencyID(searchDetails, source);

            foreach (var propertyResult in response.PropertyResults)
            {
                foreach (var roomType in propertyResult.RoomTypes)
                {
                    transformedResult.Add(new TransformedResult
                    {
                        TPKey = propertyResult.PropertyReferenceID.ToString(),
                        CurrencyCode = currency.ToSafeString(),
                        PropertyRoomBookingID = roomType.Seq,
                        RoomType = roomType.RoomTypeProperty,
                        MealBasisCode = roomType.MealBasisID.ToString(),
                        Amount = roomType.SubTotal,
                        TPReference = $"{propertyResult.BookingToken}|{roomType.RoomBookingToken}",
                        SpecialOffer = roomType.SpecialOffer
                    });
                }
            }

            return transformedResult;

        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source) => false;

        public bool ResponseHasExceptions(Request request) => false;
    }
}