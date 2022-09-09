namespace iVectorOne.Suppliers.TPIntegrationTests.HotelsProV2
{
    using System.Collections.Generic;
    using iVectorOne.Tests.HotelsProV2;
    using iVectorOne.Search.Models;
    using Helpers;
    using iVectorOne.Suppliers.HotelsProV2;
    using Xunit;

    public class HotelsProV2SearchTest : ThirdPartyPropertySearchBaseTest
    {

        private const string _provider = "HotelsProV2";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IHotelsProV2Settings _settings = new InjectedHotelsProV2Settings();

        public HotelsProV2SearchTest()
            : base(
            _provider,
            new List<SearchDetails> { _searchDetails },
            _settings,
            new HotelsProV2Search(_settings))
        {
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(await base.ValidBuildSearchRequestAsync(HotelsProV2Res.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(HotelsProV2Res.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(HotelsProV2Res.Response, HotelsProV2Res.TransformedResponse, SearchDetailsList[0]));
        }

    }
}
