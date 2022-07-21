namespace iVectorOne.Suppliers.TPIntegrationTests.JonView
{
    using Intuitive.Helpers.Serialization;
    using iVectorOne.CSSuppliers.JonView;
    using iVectorOne.Search.Models;
    using iVectorOne.Suppliers.TPIntegrationTests.Helpers;
    using iVectorOne.Tests.JonView;

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
        public async Task BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(await base.ValidBuildSearchRequestAsync(JonViewRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(JonViewRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(JonViewRes.SearchResponse, JonViewRes.TransformedResult, SearchDetailsList[0]));
        }
    }
}