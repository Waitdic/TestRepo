namespace iVectorOne.Suppliers.TPIntegrationTests.AbreuV2
{
    using System.Collections.Generic;
    using Helpers;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Suppliers.AbreuV2;
    using iVectorOne.Search.Models;
    using Xunit;

    public class AbreuV2SearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "AbreuV2";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IAbreuV2Settings _settings = new InjectedAbreuV2Settings();

        public AbreuV2SearchTest()
            : base(
            _provider,
            new List<SearchDetails> { _searchDetails },
            _settings,
            new AbreuV2Search(_settings, new Serializer()))
        {
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(await base.ValidBuildSearchRequestAsync(AbreuV2Res.RequestLogs));
            Assert.False(await base.InvalidBuildSearchRequestAsync(AbreuV2Res.RequestLogs));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(AbreuV2Res.Response, AbreuV2Res.TransformedResponse, SearchDetailsList[0]));
        }
    }
}