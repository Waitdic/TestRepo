namespace iVectorOne.Suppliers.TPIntegrationTests.HotelBedsV2
{
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive.Helpers.Security;
    using Intuitive.Helpers.Net;
    using Moq;
    using iVectorOne.Lookups;
    using iVectorOne.Search.Models;
    using iVectorOne.Suppliers.TPIntegrationTests.Helpers;
    using iVectorOne.Suppliers.HotelBedsV2;
    using iVectorOne.Tests.HotelBedsV2;

    public class HotelBedsSearchTests : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "HotelBedsV2";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IHotelBedsV2Settings _settings = new InjectedHotelBedsV2Settings();

        public HotelBedsSearchTests() : base(
            _provider,
            new List<SearchDetails>() { _searchDetails },
            _settings,
            new HotelBedsV2Search(_settings, new Mock<ITPSupport>().Object, new Mock<ISecretKeeper>().Object))
        {
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(await base.ValidBuildSearchRequestAsync(HotelBedsV2Res.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(HotelBedsV2Res.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(HotelBedsV2Res.ResponseString, HotelBedsV2Res.TransformedResultXML, SearchDetailsList[0]));
        }
    }
}