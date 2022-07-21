namespace iVectorOne.Suppliers.TPIntegrationTests.W2M
{
    using System.Collections.Generic;
    using System.Net;
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Moq.Protected;
    using iVectorOne.Suppliers;
    using iVectorOne.Search.Models;
    using iVectorOne.Suppliers.TPIntegrationTests.Helpers;
    using iVectorOne.Tests.W2M;

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
        public async Task BuiltSearchRequestTest()
        {
            Assert.True(await base.ValidBuildSearchRequestAsync(W2MRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(W2MRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Arrange, Act, Assert
            Assert.True(base.ValidTransformResponse(W2MRes.ResponseXML, W2MRes.TransformedResultXML, SearchDetailsList[0]));
        }
    }
}