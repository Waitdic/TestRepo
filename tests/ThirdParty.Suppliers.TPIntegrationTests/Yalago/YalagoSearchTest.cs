namespace ThirdParty.Suppliers.TPIntegrationTests.Yalago
{
    using System.Collections.Generic;
    using Moq;
    using ThirdParty.CSSuppliers;
    using ThirdParty.Lookups;
    using ThirdParty.Search.Models;
    using ThirdParty.Suppliers.TPIntegrationTests.Helpers;
    using ThirdParty.Tests.Yalago;

    public class YalagoSearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "Yalago";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IYalagoSettings _settings = new InjectedYalagoSettings();

        private static readonly Mock<ITPSupport> _mockSupport = SetupTPSupportMock();

        public YalagoSearchTest() : base(
                    _provider,
            new List<SearchDetails>() { _searchDetails },
            _settings,
            new YalagoSearch(_settings, _mockSupport.Object))
        {
        }

        private static Mock<ITPSupport> SetupTPSupportMock()
        {
            var mockSupport = new Mock<ITPSupport>();
            mockSupport.Setup(x => x.TPCountryCodeLookupAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult("GB"));

            return mockSupport;
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(await base.ValidBuildSearchRequestAsync(YalagoRes.RequestLogs));
            Assert.False(await base.InvalidBuildSearchRequestAsync(YalagoRes.RequestLogs));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(YalagoRes.Response, YalagoRes.TransformedResponse, SearchDetailsList[0]));
        }
    }
}
