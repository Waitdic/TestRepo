namespace ThirdParty.Suppliers.TPIntegrationTests.YouTravel
{
    using Intuitive.Helpers.Serialization;
    using Moq;
    using System.Collections.Generic;
    using ThirdParty.CSSuppliers.YouTravel;
    using ThirdParty.Lookups;
    using ThirdParty.Search.Models;
    using ThirdParty.Suppliers.TPIntegrationTests.Helpers;
    using ThirdParty.Tests.YouTravel;

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
        public void BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(base.ValidBuildSearchRequest(YouTravelRes.RequestLog));
            Assert.False(base.InvalidBuildSearchRequest(YouTravelRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(YouTravelRes.ResponseString, YouTravelRes.TransformedResponseXML, SearchDetailsList[0]));
        }
    }
}
