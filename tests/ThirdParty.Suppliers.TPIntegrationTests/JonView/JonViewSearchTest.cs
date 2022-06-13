namespace ThirdParty.Suppliers.TPIntegrationTests.JonView
{
    using Intuitive.Helpers.Serialization;
    using ThirdParty.CSSuppliers.JonView;
    using ThirdParty.Search.Models;
    using ThirdParty.Suppliers.TPIntegrationTests.Helpers;
    using ThirdParty.Tests.JonView;

    public class JonViewSearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "JonView";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IJonViewSettings _settings = new InjectedJonViewSettings();

        public JonViewSearchTest() : base(
            _provider,
            new List<SearchDetails>() { _searchDetails },
            _settings,
            new JonViewSearch(_settings, new Serializer()))
        {
        }

        [Fact]
        public void BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(base.ValidBuildSearchRequest(JonViewRes.RequestLog));
            Assert.False(base.InvalidBuildSearchRequest(JonViewRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(JonViewRes.SearchResponse, JonViewRes.TransformedResult, SearchDetailsList[0]));
        }
    }
}