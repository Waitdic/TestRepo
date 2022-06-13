namespace ThirdParty.Suppliers.TPIntegrationTests.WHL
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using CSSuppliers.Netstorming;
    using Helpers;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using Moq;
    using ThirdParty.Lookups;
    using ThirdParty.Search.Models;
    using ThirdParty.Tests.Netstorming;

    public class NetstormingSearchTests : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "WHL";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly INetstormingSettings _settings = new InjectedNetstormingSettings();

        private static readonly Mock<ITPSupport> _mockSupport = SetupTPSupportMock();

        public NetstormingSearchTests() : base(
            _provider,
            new List<SearchDetails> { _searchDetails },
            _settings,
            new NetstormingSearch(_settings, _mockSupport.Object, new Serializer()))
        {
        }

        private static Mock<ITPSupport> SetupTPSupportMock()
        {
            var mockSupport = new Mock<ITPSupport>();
            mockSupport.Setup(x => x.TPNationalityLookup(It.IsAny<string>(), It.IsAny<int>())).Returns("ES");
            return mockSupport;
        }

        public new bool ValidBuildSearchRequest(string requestLog)
        {
            // Arrange 
            var mockRequests = GetRequests(requestLog);
            var builtRequests = new List<Request>();
            const string format = "<timestamp>{0}</timestamp>";
            string pattern = string.Format(format, @"(\d+?)");
            string replacement = string.Format(format, string.Empty);

            // Act
            foreach (var searchDetails in SearchDetailsList)
            {
                var request = SearchClass.BuildSearchRequests(searchDetails, ResortSplits)[0];
                string requestString = Regex.Replace(request.RequestString, pattern, replacement); // remove timestamp
                request.SetRequest(requestString); 
                builtRequests.Add(request);
            }

            // Assert
            return Helper.AreSameWebRequests(mockRequests, builtRequests);
        }

        [Fact]
        public void BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(ValidBuildSearchRequest(NetstormingRes.RequestLog));
            Assert.False(base.InvalidBuildSearchRequest(NetstormingRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(NetstormingRes.Response, NetstormingRes.TransformedResponse, SearchDetailsList[0]));
        }
    }
}
