namespace iVectorOne.Suppliers.TPIntegrationTests.ExpediaRapid
{
    using Moq;
    using System.Collections.Generic;
    using iVectorOne.Suppliers.ExpediaRapid;
    using iVectorOne.Lookups;
    using iVectorOne.Search.Models;
    using iVectorOne.Suppliers.TPIntegrationTests.Helpers;
    using iVectorOne.Tests.ExpediaRapid;

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
            mockSupport.Setup(x => x.TPCurrencyCodeLookupAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult("GBP"));
            mockSupport.Setup(x => x.TPMealBasesAsync(It.IsAny<string>())).Returns(Task.FromResult(Helper.GetMealBasisCodes()));
            return mockSupport;
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(await base.ValidBuildSearchRequestAsync(ExpediaRapidRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(ExpediaRapidRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(ExpediaRapidRes.ResponseString, ExpediaRapidRes.TransformedResultXML, SearchDetailsList[0]));
        }
    }
}