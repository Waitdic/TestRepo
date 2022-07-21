namespace ThirdParty.Suppliers.TPIntegrationTests.ATI
{
    using System.Collections.Generic;
    using Intuitive.Helpers.Serialization;
    using ThirdParty.CSSuppliers.ATI;
    using ThirdParty.Search.Models;
    using ThirdParty.Suppliers.TPIntegrationTests.Helpers;
    using Xunit;

    public class ATISearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "ATI";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IATISettings _settings = new InjectedATISettings();

        public ATISearchTest() : base(
            _provider,
            new List<SearchDetails>() { _searchDetails },
            _settings,
            new ATISearch(_settings, new Serializer(), null!))
        {
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(await base.ValidBuildSearchRequestAsync(ATIRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(ATIRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(ATIRes.ResponseXML, ATIRes.TransformedResultXML, SearchDetailsList[0]));
        }
    }
}