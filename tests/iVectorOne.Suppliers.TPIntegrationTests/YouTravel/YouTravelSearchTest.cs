namespace iVectorOne.Suppliers.TPIntegrationTests.YouTravel
{
    using Intuitive.Helpers.Serialization;
    using Moq;
    using System.Collections.Generic;
    using iVectorOne.Suppliers.YouTravel;
    using iVectorOne.Lookups;
    using iVectorOne.Search.Models;
    using iVectorOne.Suppliers.TPIntegrationTests.Helpers;
    using iVectorOne.Tests.YouTravel;

    public class YouTravelSearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "YouTravel";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IYouTravelSettings _settings = new InjectedYouTravelSettings();

        public YouTravelSearchTest() : base(
            _provider,
            new List<SearchDetails>() { _searchDetails },
            _settings,
            new YouTravelSearch(_settings, new Serializer()))
        {
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            Assert.True(await base.ValidBuildSearchRequestAsync(YouTravelRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(YouTravelRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(YouTravelRes.ResponseString, YouTravelRes.TransformedResponseXML, SearchDetailsList[0]));
        }
    }
}
