namespace iVectorOne.Suppliers.TPIntegrationTests.Serhs
{
    using System.Collections.Generic;
    using Helpers;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Suppliers.Serhs;
    using iVectorOne.Search.Models;
    using iVectorOne.Tests.Serhs;

    public class SerhsSearchTests : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "SERHS";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly ISerhsSettings _settings = new InjectedSerhsSettings();

        public SerhsSearchTests() : base(
            _provider,
            new List<SearchDetails> { _searchDetails },
            _settings,
            new SerhsSearch(_settings, new Serializer()))
        {
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            Assert.True(await base.ValidBuildSearchRequestAsync(SerhsRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(SerhsRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(SerhsRes.Response, SerhsRes.TransformedResponse, SearchDetailsList[0]));
        }
    }
}
