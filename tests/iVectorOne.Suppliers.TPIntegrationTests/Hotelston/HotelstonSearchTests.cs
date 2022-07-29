namespace iVectorOne.Suppliers.TPIntegrationTests.Hotelston
{
    using System.Collections.Generic;
    using iVectorOne.Lookups;
    using iVectorOne.Search.Models;
    using Suppliers.Hotelston;
    using Helpers;
    using Xunit;
    using Intuitive.Helpers.Serialization;

    public class HotelstonSearchTests : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "Hotelston";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IHotelstonSettings _settings = new InjectedHotelstonSettings();

        public HotelstonSearchTests() : base(
            _provider,
            new List<SearchDetails> { _searchDetails },
            _settings,
            new HotelstonSearch(_settings, new Serializer()))
        {
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(await base.ValidBuildSearchRequestAsync(HotelstonRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(HotelstonRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(HotelstonRes.Response, HotelstonRes.TransformedResponse, SearchDetailsList[0]));
        }
    }
}