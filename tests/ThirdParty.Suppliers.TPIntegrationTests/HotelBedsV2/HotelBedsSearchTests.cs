namespace ThirdParty.Suppliers.TPIntegrationTests.HotelBedsV2
{
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive.Helpers.Security;
    using Intuitive.Net.WebRequests;
    using Moq;
    using ThirdParty.Lookups;
    using ThirdParty.Search.Models;
    using ThirdParty.Suppliers.TPIntegrationTests.Helpers;
    using ThirdParty.CSSuppliers.HotelBedsV2;
    using ThirdParty.Tests.HotelBedsV2;

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

        public override List<Request> GetRequests(string requestLog)
        {
            List<Request> requests = base.GetRequests(requestLog);

            for (int i = 0; i < SearchDetailsList.Count; ++i)
            {
                var searchHelper = new SearchHelper
                {
                    SearchDetails = SearchDetailsList[i],
                    ResortSplit = ResortSplits.FirstOrDefault(),
                    UniqueRequestID = "HotelBedsV2"
                };

                requests[i].ExtraInfo = searchHelper;
            }

            return requests;
        }

        [Fact]
        public void BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(base.ValidBuildSearchRequest(HotelBedsV2Res.RequestLog));
            Assert.False(base.InvalidBuildSearchRequest(HotelBedsV2Res.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Arrange 
            var searchHelper = new SearchHelper
            {
                SearchDetails = SearchDetailsList[0],
                ResortSplit = ResortSplits.FirstOrDefault(),
                UniqueRequestID = "HotelBedsV2"
            };

            // Assert 
            Assert.True(base.ValidTransformResponse(HotelBedsV2Res.ResponseString, HotelBedsV2Res.TransformedResultXML, SearchDetailsList[0], searchHelper));
        }
    }
}