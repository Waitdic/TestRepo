namespace iVectorOne.Suppliers.TPIntegrationTests.Jumbo
{
    using System.Collections.Generic;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.CSSuppliers.Jumbo;
    using iVectorOne.Search.Models;
    using iVectorOne.Suppliers.TPIntegrationTests.Helpers;
    using iVectorOne.Tests.Jumbo;

    public class JumboSearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "Jumbo";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IJumboSettings _settings = new InjectedJumboSettings();

        public JumboSearchTest() : base(
            _provider,
            new List<SearchDetails>() { _searchDetails },
            _settings,
            new JumboSearch(_settings, new Serializer()))
        {
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(await base.ValidBuildSearchRequestAsync(JumboRes.RequestLogs));
            Assert.False(await base.InvalidBuildSearchRequestAsync(JumboRes.RequestLogs));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(JumboRes.Response, JumboRes.TransformedResponse, SearchDetailsList[0]));
        }
    }
}