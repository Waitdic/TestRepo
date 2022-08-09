namespace iVectorOne.Suppliers.TPIntegrationTests.GoGlobal
{
    using System.Collections.Generic;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Search.Models;
    using iVectorOne.Suppliers.TPIntegrationTests.Helpers;
    using iVectorOne.Suppliers.GoGlobal;
    using iVectorOne.Tests.GoGlobal;

    public class GoGlobalSearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "GoGlobal";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IGoGlobalSettings _settings = new InjectedGoGlobalSettings();

        private static readonly string iRoomIdx = "1";

        public GoGlobalSearchTest() : base
            (
                _provider,
                new List<SearchDetails>() { _searchDetails },
                _settings,
                new GoGlobalSearch(_settings, new Serializer())
            )
        {

        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            Assert.True(await base.ValidBuildSearchRequestAsync(GoGlobalRes.RequestLogs));
            Assert.False(await base.InvalidBuildSearchRequestAsync(GoGlobalRes.RequestLogs));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(GoGlobalRes.Response, GoGlobalRes.TransformedResponse, SearchDetailsList[0], iRoomIdx));
        }
    }
}
