namespace ThirdParty.Suppliers.TPIntegrationTests.DOTW
{
    using System.Collections.Generic;
    using Intuitive.Helpers.Serialization;
    using Moq;
    using ThirdParty.CSSuppliers.DOTW;
    using ThirdParty.Lookups;
    using ThirdParty.Search.Models;
    using ThirdParty.Suppliers.TPIntegrationTests.Helpers;
    using ThirdParty.Tests.DOTW;

    public class DOTWSearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "DOTW";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IDOTWSettings _settings = new InjectedDOTWSettings();

        private static readonly Mock<ITPSupport> _mockSupport = SetupTPSupportMock();

        private static readonly Mock<IDOTWSupport> _dotwSupoprtMock = SetupDOTWMockSupport();

        public DOTWSearchTest() : base(
            _provider, new List<SearchDetails>() { _searchDetails },
            _settings, new DOTWSearch(_settings, _mockSupport.Object, _dotwSupoprtMock.Object, new Serializer()))
        {
        }

        private static Mock<ITPSupport> SetupTPSupportMock()
        {
            var mockSupport = new Mock<ITPSupport>();
            mockSupport.Setup(x => x.TPCurrencyLookup(It.IsAny<string>(), It.IsAny<string>())).Returns("100");
            mockSupport.Setup(x => x.TPNationalityLookup(It.IsAny<string>(), It.IsAny<string>())).Returns("100");
            return mockSupport;
        }

        private static Mock<IDOTWSupport> SetupDOTWMockSupport()
        {
            var dotwSupport = new Mock<IDOTWSupport>();
            dotwSupport.Setup(x => x.GetCurrencyID(It.IsAny<IThirdPartyAttributeSearch>())).Returns(100);

            return dotwSupport;
        }

        [Fact]
        public void BuiltSearchRequestTest()
        {
            // Arrange 
            ResortSplits = Helper.CreateResortSplits(Supplier, Helper.CreateHotels(2795805, 49886, "Torre Hogar", "102245"), "15304|140965");

            // Assert 
            Assert.True(base.ValidBuildSearchRequest(DOTWRes.RequestLog));
            Assert.False(base.InvalidBuildSearchRequest(DOTWRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(DOTWRes.Response, DOTWRes.TransformedResponse, SearchDetailsList[0]));
        }
    }
}