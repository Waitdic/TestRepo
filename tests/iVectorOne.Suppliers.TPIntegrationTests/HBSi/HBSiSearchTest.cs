namespace iVectorOne.Suppliers.TPIntegrationTests.HBSi
{
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Search.Models;
    using iVectorOne.Suppliers.HBSi;
    using iVectorOne.Suppliers.TPIntegrationTests;
    using iVectorOne.Suppliers.TPIntegrationTests.Helpers;
    using iVectorOne.Tests.HBSi;
    using System.Collections.Generic;

    public class HBSiSearchTest : ThirdPartyPropertySearchBaseTest
    {

        private const string _provider = "HBSiSandals";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IHBSiSettings _settings = new InjectedHBSiSettings();

        public HBSiSearchTest() : base(
           _provider,
           new List<SearchDetails>() { _searchDetails },
           _settings,
           new HBSiSearch(_settings, new Serializer()))
        {
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(await base.ValidBuildSearchRequestAsync(HBSiRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(HBSiRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(HBSiRes.Response, HBSiRes.TransformedResponse, SearchDetailsList[0]));
        }
    }
}
