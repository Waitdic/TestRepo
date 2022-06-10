namespace ThirdParty.Suppliers.TPIntegrationTests.Juniper
{
    using System.Collections.Generic;
    using Intuitive.Helpers.Serialization;
    using ThirdParty.CSSuppliers.Juniper;
    using ThirdParty.Constants;
    using ThirdParty.Search.Models;
    using ThirdParty.Suppliers.TPIntegrationTests.Helpers;
    using ThirdParty.Tests.Juniper;

    public class JuniperSearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = ThirdParties.JUNIPERELEVATE;

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IJuniperSettings _settings = new InjectedJuniperSettings();

        public JuniperSearchTest() : base(
            _provider,
            new List<SearchDetails> { _searchDetails },
            _settings,
            new JuniperSearch(_settings, new Serializer()))
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