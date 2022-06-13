namespace ThirdParty.Suppliers.TPIntegrationTests.MTS
{
    using Intuitive.Helpers.Serialization;
    using System.Collections.Generic;
    using ThirdParty.CSSuppliers.MTS;
    using ThirdParty.Search.Models;
    using ThirdParty.Suppliers.TPIntegrationTests.Helpers;
    using ThirdParty.Tests.MTS;

    public class MTSSearchTests : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "MTS";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IMTSSettings _settings = new InjectedMTSSettings();

        public MTSSearchTests() : base(
            _provider,
            new List<SearchDetails>() { _searchDetails },
            _settings,
            new MTSSearch(_settings, new Serializer()))
        {
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            Assert.True(await base.ValidBuildSearchRequestAsync(MTSRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(MTSRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(MTSRes.ResponseXML, MTSRes.TransformedResultXML, SearchDetailsList[0]));
        }
    }
}