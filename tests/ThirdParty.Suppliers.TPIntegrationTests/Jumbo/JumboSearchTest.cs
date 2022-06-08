namespace ThirdParty.Suppliers.TPIntegrationTests.Jumbo
{
    using System.Collections.Generic;
    using Intuitive.Helpers.Serialization;
    using ThirdParty.CSSuppliers.Jumbo;
    using ThirdParty.Search.Models;
    using ThirdParty.Suppliers.TPIntegrationTests.Helpers;
    using ThirdParty.Tests.Jumbo;

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
        public void BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(base.ValidBuildSearchRequest(JumboRes.RequestLogs));
            Assert.False(base.InvalidBuildSearchRequest(JumboRes.RequestLogs));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(JumboRes.Response, JumboRes.TransformedResponse, SearchDetailsList[0]));
        }
    }
}