namespace ThirdParty.Suppliers.TPIntegrationTests.WHL
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using ThirdParty.Lookups;
    using ThirdParty.Search.Models;
    using CSSuppliers.NetStorming;
    using CSSuppliers.NetStorming.WHL;
    using Intuitive.Net.WebRequests;
    using Moq;
    using Helpers;
    using Intuitive.Helpers.Serialization;
    using ThirdParty.Tests.WHL;

    public class WHLSearchTests : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "WHL";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly INetstormingSettings _settings = new InjectedWHLSettings();

        private static readonly Mock<ITPSupport> _mockSupport = SetupTPSupportMock();

        public WHLSearchTests() : base(
            _provider,
            new List<SearchDetails> { _searchDetails },
            _settings,
            new WHLSearch(_settings, _mockSupport.Object, new Serializer()))
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
            Assert.True(ValidBuildSearchRequest(WHLRes.RequestLog));
            Assert.False(base.InvalidBuildSearchRequest(WHLRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(WHLRes.Response, WHLRes.TransformedResponse, SearchDetailsList[0]));
        }
    }
}
