namespace iVectorOne.Suppliers.TPIntegrationTests.Polaris
{
    using System.Collections.Generic;
    using Helpers;
    using Intuitive.Helpers.Security;
    using iVectorOne.Suppliers.Polaris;
    using iVectorOne.Search.Models;
    using iVectorOne.Tests.Polaris;

    public class PolarisSearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "Polaris";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IPolarisSettings _settings = new InjectedPolarisSettings();

        public PolarisSearchTest() : base(
            _provider,
            new List<SearchDetails> { _searchDetails },
            _settings,
            new PolarisSearch(_settings, new SecretKeeper("testtest")))
        {
        }


        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(await base.ValidBuildSearchRequestAsync(PolarisRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(PolarisRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(PolarisRes.Response, PolarisRes.TransformedResponse, SearchDetailsList[0]));
        }
    }
}
