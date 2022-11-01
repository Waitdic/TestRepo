namespace iVectorOne.Suppliers.TPIntegrationTests.TBOHolidays
{
    using System.Collections.Generic;
    using Helpers;
    using Intuitive.Helpers.Serialization;
    using Moq;
    using iVectorOne.Suppliers.TBOHolidays;
    using iVectorOne.Lookups;
    using iVectorOne.Search.Models;
    using Xunit;
    using iVectorOne.Tests.TBOHolidays;

    public class TBOHolidaysSearchTests : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "TBOHolidays";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly ITBOHolidaysSettings _settings = new InjectedTBOHolidaysSettings();

        public TBOHolidaysSearchTests() : base(
            _provider,
            new List<SearchDetails> { _searchDetails },
            _settings,
            new TBOHolidaysSearch(_settings, new Mock<ITPSupport>().Object, new Serializer()))
        {
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            ResortSplits.ForEach(r => r.ResortCode = $"|{r.ResortCode}|");

            // Assert 
            Assert.True(await base.ValidBuildSearchRequestAsync(TBOHolidaysRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(TBOHolidaysRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(TBOHolidaysRes.ResponseString, TBOHolidaysRes.TransformedResultXML, SearchDetailsList[0]));
        }
    }
}
