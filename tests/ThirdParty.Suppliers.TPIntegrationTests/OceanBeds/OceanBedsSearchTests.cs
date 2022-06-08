namespace ThirdParty.Suppliers.TPIntegrationTests.OceanBeds
{
    using System.Collections.Generic;
    using ThirdParty.Search.Models;
    using ThirdParty.CSSuppliers.OceanBeds;
    using ThirdParty.Suppliers.TPIntegrationTests.Helpers;
    using Xunit;
    using Intuitive.Helpers.Serialization;
    using ThirdParty.Tests.OceanBeds;

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
        public void BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(base.ValidBuildSearchRequest(OceanBedsRes.RequestLog));
            Assert.False(base.InvalidBuildSearchRequest(OceanBedsRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(OceanBedsRes.Response, OceanBedsRes.TransformedResponse, SearchDetailsList[0]));
        }
    }
}

