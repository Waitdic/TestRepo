namespace ThirdParty.Suppliers.TPIntegrationTests.JuniperElevate
{
    using System.Collections.Generic;
    using ThirdParty.Lookups;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Support;
    using ThirdParty.Suppliers.TPIntegrationTests.Helpers;
    using Xunit;
    using ThirdParty.CSSuppliers.Juniper;
    using Intuitive.Helpers.Serialization;
    using ThirdParty.Tests.JuniperElevate;

    public class JuniperElevateSearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "JuniperElevate";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IJuniperElevateSettings _settings = new InjectedJuniperElevateSettings();

        public JuniperElevateSearchTest() : base(
            _provider,
            new List<SearchDetails> { _searchDetails },
            _settings,
            new JuniperElevateSearch(_settings, new Serializer()))
        {
        }

        [Fact]
        public void BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(base.ValidBuildSearchRequest(JuniperElevateRes.RequestLog));
            Assert.False(base.InvalidBuildSearchRequest(JuniperElevateRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(JuniperElevateRes.Response, JuniperElevateRes.TransformedResponse, SearchDetailsList[0]));
        }
    }
}