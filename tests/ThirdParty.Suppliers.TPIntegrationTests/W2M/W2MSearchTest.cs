namespace ThirdParty.Suppliers.TPIntegrationTests.W2M
{
    using System.Collections.Generic;
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using Moq;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Support;
    using ThirdParty.CSSuppliers;
    using ThirdParty.Suppliers.TPIntegrationTests.Helpers;
    using ThirdParty.Tests.W2M;
    using System.Net;
    using Moq.Protected;

    public class W2MSearchTests : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "W2M";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IW2MSettings _settings = new InjectedW2MSettings();

        private static readonly HttpClient _httpClient = SetupHttpClient();

        public W2MSearchTests() : base(
            _provider,
            new List<SearchDetails>() { _searchDetails },
            _settings,
            new W2MSearch(_settings, new Serializer(), _httpClient, new Mock<ILogger<W2MSearch>>().Object))
        {
        }

        private static HttpClient SetupHttpClient()
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"[{ ""id"": 1, ""title"": ""Cool post!""}, { ""id"": 100, ""title"": ""Some title""}]"),
            };
            var mockHttp = new Mock<HttpMessageHandler>();
            mockHttp
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            return new HttpClient(mockHttp.Object);
        }

        [Fact]
        public void BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(base.ValidBuildSearchRequest(W2MRes.RequestLog));
            Assert.False(base.InvalidBuildSearchRequest(W2MRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Arrange
            var searchHelper = new SearchExtraHelper
            {
                SearchDetails = SearchDetailsList[0],
                UniqueRequestID = "W2M"
            };

            // Assert 
            Assert.True(base.ValidTransformResponse(W2MRes.ResponseXML, W2MRes.TransformedResultXML, SearchDetailsList[0], searchHelper));
        }
    }
}