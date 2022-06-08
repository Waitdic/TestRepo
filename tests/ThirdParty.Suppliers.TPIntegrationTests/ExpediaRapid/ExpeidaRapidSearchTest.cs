namespace ThirdParty.Suppliers.TPIntegrationTests.ExpediaRapid
{
    using Moq;
    using System.Collections.Generic;
    using ThirdParty.CSSuppliers.ExpediaRapid;
    using ThirdParty.Lookups;
    using ThirdParty.Search.Models;
    using ThirdParty.Suppliers.TPIntegrationTests.Helpers;
    using ThirdParty.Tests.ExpediaRapid;

    public class ExpeidaRapidSearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "ExpediaRapid";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IExpediaRapidSettings _settings = new InjectedExpediaRapidSettings();

        private static readonly Mock<ITPSupport> _mockSupport = SetupTPSupportMock();

        public ExpeidaRapidSearchTest() : base(
            _provider,
            new List<SearchDetails>() { _searchDetails },
            _settings,
            new ExpediaRapidSearch(_settings, _mockSupport.Object))
        {
        }

        private static Mock<ITPSupport> SetupTPSupportMock()
        {
            var mockSupport = new Mock<ITPSupport>();
            mockSupport.Setup(x => x.TPCurrencyLookup(It.IsAny<string>(), It.IsAny<string>())).Returns("GBP");
            mockSupport.Setup(x => x.TPMealBases(It.IsAny<string>())).Returns(Helper.GetMealBasisCodes());
            return mockSupport;
        }

        [Fact]
        public void BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(base.ValidBuildSearchRequest(ExpediaRapidRes.RequestLog));
            Assert.False(base.InvalidBuildSearchRequest(ExpediaRapidRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(ExpediaRapidRes.ResponseString, ExpediaRapidRes.TransformedResultXML, SearchDetailsList[0]));
        }
    }
}