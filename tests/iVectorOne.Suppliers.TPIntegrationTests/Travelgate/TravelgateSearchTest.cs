//namespace ThirdParty.Suppliers.TPIntegrationTests.Travelgate
//{
//    using System.Collections.Generic;
//    using Moq;
//    using ThirdParty.Abstractions.Lookups;
//    using ThirdParty.Abstractions.Search.Models;
//    using ThirdParty.Suppliers.TPIntegrationTests.Helpers;
//    using ThirdParty.VBSuppliers;
//    using Xunit;
//    public class TravelgateSearchTest : ThirdPartyPropertySearchBaseTest
//    {
//        private const string _provider = "Travelgate";

//        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

//        private static readonly ITravelgateSettings _settings = new InjectedTravelgateSettings(_searchDetails.ThirdPartyConfigurations[0]);

//        private static readonly Mock<ITPSupport> _mockSupport = SetupTPSupportMock();

//        public TravelgateSearchTest() : base(
//           _provider,
//           new List<SearchDetails>() { _searchDetails },
//           _settings,
//           new TravelgateSearch(_settings, _mockSupport.Object))
//        {
//        }

//        private static Mock<ITPSupport> SetupTPSupportMock()
//        {
//            var mockSupport = new Mock<ITPSupport>();
//            mockSupport.Setup(x => x.TPNationalityLookup(It.IsAny<string>(), It.IsAny<int>())).Returns("ES");
//            return mockSupport;
//        }

//        [Fact]
//        public void BuiltSearchRequestTest()
//        {
//            // Assert 
//            Assert.True(base.ValidBuildSearchRequest(TravelgateRes.RequestLog));
//            Assert.False(base.InvalidBuildSearchRequest(TravelgateRes.RequestLog));
//        }

//        [Fact]
//        public void TransformResponseTest()
//        {
//            // Assert 
//            Assert.True(base.ValidTransformResponse(TravelgateRes.Response, TravelgateRes.TransformedResponse, SearchDetailsList[0]));
//        }
//    }
//}
