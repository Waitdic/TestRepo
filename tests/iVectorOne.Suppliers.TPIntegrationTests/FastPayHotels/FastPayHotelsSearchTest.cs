namespace iVectorOne.Suppliers.TPIntegrationTests.Fastpayhotels
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Moq;
    using Intuitive.Helpers.Net;
    using iVectorOne.Lookups;
    using iVectorOne.Search.Models;
    using iVectorOne.Suppliers.TPIntegrationTests.Helpers;
    using iVectorOne.Suppliers.FastPayHotels;
    using Xunit;

    public class FastPayHotelsSearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "Fastpayhotels";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IFastPayHotelsSettings _settings = new InjectedFastPayHotelsSettings();

        private static readonly Mock<ITPSupport> _mockSupport = SetupTPSupportMock();

        public FastPayHotelsSearchTest() : base(
           _provider,
           new List<SearchDetails>() { _searchDetails },
           _settings,
           new FastPayHotelsSearch(_settings, _mockSupport.Object))
        {
        }

        private static Mock<ITPSupport> SetupTPSupportMock()
        {
            var mockSupport = new Mock<ITPSupport>();
            mockSupport.Setup(x => x.TPNationalityLookupAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult("GB"));
            mockSupport.Setup(x => x.TPCountryCodeLookupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult("GB"));
            return mockSupport;
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            // Arrange 
            var messageID = "Test1234";
            var requests = new List<Request>();
            var searchRequests = GetRequests(FastPayHotelsRes.RequestLogs);

            // Act 
            for (int i = 0; i < SearchDetailsList.Count; ++i)
            {
                var availabilityRequest = await ((FastPayHotelsSearch)SearchClass).CreateAvailabilityRequestAsync(SearchDetailsList[i], SearchDetailsList[i].RoomDetails, messageID, ResortSplits);
                string requestString = JsonConvert.SerializeObject(availabilityRequest);
                var request = new Request()
                {
                    EndPoint = _settings.AvailabilityURL(SearchDetailsList[0]) + "api/booking/availability",
                    Source = _provider
                };

                request.SetRequest(requestString);
                requests.Add(request);
            }


            // Assert 
            Assert.True(Helper.AreSameWebRequests(requests, searchRequests));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(FastPayHotelsRes.Response, FastPayHotelsRes.TransformedResponse, SearchDetailsList[0]));
        }
    }
}