namespace ThirdParty.Suppliers.TPIntegrationTests.Stuba
{
    using System.Collections.Generic;
    using Intuitive.Helpers.Serialization;
    using ThirdParty.CSSuppliers.Stuba;
    using ThirdParty.Search.Models;
    using ThirdParty.Suppliers.TPIntegrationTests.Helpers;
    using ThirdParty.Tests.Stuba;

    public class StubaSearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "Stuba";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IStubaSettings _settings = new InjectedStubaSettings();

        public StubaSearchTest() : base(
            _provider,
            new List<SearchDetails>() { _searchDetails },
            _settings,
            new StubaSearch(_settings, new Serializer()))
        {
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            Assert.True(await base.ValidBuildSearchRequestAsync(StubaRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(StubaRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(StubaRes.ResponseString, StubaRes.TransformedResultXML, SearchDetailsList[0]));
        }
    }
}