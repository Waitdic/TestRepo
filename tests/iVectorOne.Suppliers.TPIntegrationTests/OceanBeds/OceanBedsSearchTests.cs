namespace iVectorOne.Suppliers.TPIntegrationTests.OceanBeds
{
    using System.Collections.Generic;
    using iVectorOne.Search.Models;
    using iVectorOne.Suppliers.OceanBeds;
    using iVectorOne.Suppliers.TPIntegrationTests.Helpers;
    using Xunit;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Tests.OceanBeds;

    public class OceanBedsResSearchTests : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "OceanBeds";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IOceanBedsSettings _settings = new InjectedOceanBedsSettings();

        public OceanBedsResSearchTests() : base(
            _provider,
            new List<SearchDetails> { _searchDetails },
            _settings,
            new OceanBedsSearch(_settings, new Serializer()))
        {
            ResortSplits = Helper.CreateResortSplits(Supplier, Helper.CreateHotels(2795805, 49886, "Torre Hogar", "102245"), "1|2|3");
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            Assert.True(await base.ValidBuildSearchRequestAsync(OceanBedsRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(OceanBedsRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(OceanBedsRes.Response, OceanBedsRes.TransformedResponse, SearchDetailsList[0]));
        }
    }
}

