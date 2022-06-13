namespace ThirdParty.Suppliers.TPIntegrationTests.BedsWithEase
{
    using ThirdParty.CSSuppliers.BedsWithEase;
    using System.Collections.Generic;
    using Intuitive.Helpers.Serialization;
    using Moq;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Search.Models;
    using ThirdParty.Suppliers.TPIntegrationTests.Helpers;
    using ThirdParty.Tests.BedsWithEase;
    using Moq.Protected;
    using System.Net;

    public class BedsWithEaseSearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "BedsWithEase";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IBedsWithEaseSettings _settings = new InjectedBedsWithEaseSettings();

        private static readonly HttpClient _httpClient = SetupHttpClient();

        public BedsWithEaseSearchTest() : base(
            _provider,
            new List<SearchDetails>() { _searchDetails },
            _settings,
            new BedsWithEaseSearch(_settings, new Serializer(), _httpClient, new Mock<ILogger<BedsWithEaseSearch>>().Object))
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
            // Assert 
            Assert.True(await base.ValidBuildSearchRequestAsync(BedsWithEaseRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(BedsWithEaseRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(BedsWithEaseRes.ResponseXML, BedsWithEaseRes.TransformedResultXML, SearchDetailsList[0]));
        }
    }
}