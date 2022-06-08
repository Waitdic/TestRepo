namespace ThirdParty.Suppliers.TPIntegrationTests.TeamAmerica
{
    using System.Collections.Generic;
    using Helpers;
    using Intuitive.Helpers.Serialization;
    using ThirdParty.Search.Models;
    using ThirdParty.CSSuppliers.TeamAmerica;
    using ThirdParty.Tests.TeamAmerica;

    public class TeamAmericaSearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "TeamAmerica";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly ITeamAmericaSettings _settings = new InjectedTeamAmericaSettings();

        public TeamAmericaSearchTest()
            : base(
            _provider,
            new List<SearchDetails> { _searchDetails },
            _settings,
            new TeamAmericaSearch(_settings, new Serializer()))
        {
        }

        [Fact]
        public void BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(base.ValidBuildSearchRequest(TeamAmericaRes.RequestLog));
            Assert.False(base.InvalidBuildSearchRequest(TeamAmericaRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(TeamAmericaRes.Response, TeamAmericaRes.TransformedResponse, SearchDetailsList[0]));
        }
    }
}