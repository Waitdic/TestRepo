namespace ThirdParty.Suppliers.TPIntegrationTests.RMI
{
    using System.Collections.Generic;
    using Helpers;
    using Intuitive.Helpers.Serialization;
    using ThirdParty.CSSuppliers.RMI;
    using ThirdParty.Lookups;
    using ThirdParty.Search.Models;
    using Xunit;

    public class RMISearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "RMI";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IRMISettings _settings = new InjectedRMISettings();

        public RMISearchTest()
            : base(
            _provider,
            new List<SearchDetails> { _searchDetails },
            _settings,
            new RMISearch(_settings, new Serializer()))
        {
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(await base.ValidBuildSearchRequestAsync(RMIRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(RMIRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(RMIRes.Response, RMIRes.TransformedResponse, SearchDetailsList[0]));
        }
    }
}