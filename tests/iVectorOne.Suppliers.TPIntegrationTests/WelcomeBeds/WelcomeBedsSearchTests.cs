namespace iVectorOne.Suppliers.TPIntegrationTests.WelcomeBeds
{
    using System.Collections.Generic;
    using Intuitive.Helpers.Serialization;
    using Moq;
    using iVectorOne.CSSuppliers;
    using iVectorOne.Lookups;
    using iVectorOne.Search.Models;
    using iVectorOne.Suppliers.TPIntegrationTests.Helpers;
    using iVectorOne.Tests.WelcomeBeds;

    public class WelcomeBedsSearchTests : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "WelcomeBeds";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider, Helper.RoomWithTwoAdults);

        private static readonly IWelcomeBedsSettings _settings = new InjectedWelcomeBedsSettings();

        public WelcomeBedsSearchTests() : base(
            _provider,
            new List<SearchDetails> { _searchDetails },
            _settings,
            new WelcomeBedsSearch(_settings, new Serializer()))
        {
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            ResortSplits = Helper.CreateResortSplits(Supplier, Helper.CreateHotels(2795805, 49886, "Torre Hogar", "102245"), "15304|140965");

            // Assert 
            Assert.True(await base.ValidBuildSearchRequestAsync(WelcomeBedsRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(WelcomeBedsRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(WelcomeBedsRes.Response, WelcomeBedsRes.TransformedResponse, SearchDetailsList[0]));
        }
    }
}
