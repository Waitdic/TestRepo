namespace ThirdParty.Suppliers.TPIntegrationTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Xml;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using iVector.Search.Property;
    using Moq;
    using ThirdParty.Models;
    using ThirdParty.Search.Models;
    using ThirdParty.Suppliers.TPIntegrationTests.Helpers;

    /// <summary>
    /// Third Party Property Search Base Test
    /// </summary>
    public class ThirdPartyPropertySearchBaseTest
    {
        #region Properties

        public List<SearchDetails> SearchDetailsList { get; set; } = new(); // list of searchDetails to store different RoomDetails 
        public object Settings { get; set; }
        public IThirdPartySearch SearchClass { get; set; }
        public string Supplier { get; set; }
        public List<ResortSplit> ResortSplits { get; set; }

        #endregion

        #region Constructor

        public ThirdPartyPropertySearchBaseTest(
            string supplier,
            List<SearchDetails> searchDetailsList,
            object settings,
            IThirdPartySearch searchClass)
        {
            Supplier = supplier;
            SearchDetailsList = searchDetailsList;
            Settings = settings;
            SearchClass = searchClass;
            ResortSplits = Helper.CreateResortSplits(Supplier, Helper.CreateHotels(2795805, 49886, "Torre Hogar", "102245"), "MAD_12");
        }

        #endregion

        #region Build request test

        public virtual List<Request> GetRequests(string requestLog)
        {
            var requests = new List<Request>();
            var requestInfo = Helper.LoadRequestLog(requestLog);
            var urls = requestInfo.Item1;
            var requestStrings = requestInfo.Item2;

            SearchDetailsList.Clear();

            for (int i = 0; i < Helper.Rooms.Count(); ++i)
            {
                SearchDetailsList.Add(Helper.CreateSearchDetails(Supplier, Helper.Rooms[i]));

                var request = Helper.CreateWebRequest(urls[i], Supplier);
                request.SetRequest(requestStrings[i]);

                requests.Add(request);
            }

            return requests;
        }

        public bool ValidBuildSearchRequest(string requestLog)
        {
            // Arrange 
            var mockRequests = GetRequests(requestLog);
            var builtRequests = new List<Request>();

            // Act
            for (int i = 0; i < SearchDetailsList.Count(); ++i)
            {
                builtRequests.Add(SearchClass.BuildSearchRequests(SearchDetailsList[i], ResortSplits)[0]);
            }
            var resStr = builtRequests.Aggregate("", (all, item) => $"{all}{item.RequestLog}\n");
            // Assert
            return Helper.AreSameWebRequests(mockRequests, builtRequests);
        }

        public bool InvalidBuildSearchRequest(string requestLog)
        {
            // Arrange
            var emptyResortSplits = new List<ResortSplit>();

            // Act 
            var builtRequests = SearchClass.BuildSearchRequests(SearchDetailsList[0], emptyResortSplits);

            // Assert
            return Helper.AreSameWebRequests(GetRequests(requestLog), builtRequests);
        }

        #endregion

        #region Transform response test

        public bool ValidTransformResponse(string response, string mockResponse, SearchDetails searchDetails)
        {
            return ValidTransformResponse(response, mockResponse, searchDetails, null!, Helper.CreateResortSplits(Supplier, new List<Hotel>(), string.Empty));
        }

        public bool ValidTransformResponse(string response, string mockResponse, SearchDetails searchDetails, object extraInfo)
        {
            return ValidTransformResponse(response, mockResponse, searchDetails, extraInfo, Helper.CreateResortSplits(Supplier, new List<Hotel>(), string.Empty));
        }

        public bool ValidTransformResponse(string response, string mockResponse, SearchDetails searchDetails, object extraInfo, List<ResortSplit> resortSplits)
        {
            // Arrange 
            var request = CreateResponseWebRequest(response, searchDetails, extraInfo);

            // Act
            var transformedResponse = SearchClass.TransformResponse(new List<Request> { request }, searchDetails, resortSplits);
            var serializer = new Serializer();

            // Assert
            return Helper.IsValidResponse(mockResponse, serializer.Serialize(transformedResponse).InnerXml);
        }

        public Request CreateResponseWebRequest(string responseString, object searchDetails, object extraInfo)
        {
            var request = new Request
            {
                ExtraInfo = extraInfo ?? searchDetails,
            };

            request.SetResponse(responseString);

            return request;
        }

        #endregion
    }
}