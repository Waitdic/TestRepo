namespace iVectorOne.Suppliers.TPIntegrationTests.Juniper
{
    using System.Collections.Generic;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Suppliers.Juniper;
    using iVectorOne.Constants;
    using iVectorOne.Search.Models;
    using iVectorOne.Suppliers.TPIntegrationTests.Helpers;
    using iVectorOne.Tests.Juniper;

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
        public async Task BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(await base.ValidBuildSearchRequestAsync(JuniperElevateRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(JuniperElevateRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(JuniperElevateRes.Response, JuniperElevateRes.TransformedResponse, SearchDetailsList[0]));
        }
    }
}