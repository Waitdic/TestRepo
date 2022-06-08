namespace ThirdParty.Suppliers.TPIntegrationTests.BookABed
{
    using System.Collections.Generic;
    using Intuitive.Helpers.Serialization;
    using ThirdParty.CSSuppliers.iVectorConnect;
    using ThirdParty.Search.Models;
    using ThirdParty.Tests.BookABed;

    public class BookABedSearchTests : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "BookABed";

        private static readonly SearchDetails _searchDetails = Helpers.Helper.CreateSearchDetails(_provider);

        private static readonly IBookabedSettings _settings = new InjectedBookabedSettings();

        public BookABedSearchTests() : base(
            _provider,
            new List<SearchDetails>() { _searchDetails },
            _settings,
            new BookaBedSearch(_settings, new Serializer()))
        {
        }

        [Fact]
        public void BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(base.ValidBuildSearchRequest(BookABedRes.RequestLog));
            Assert.False(base.InvalidBuildSearchRequest(BookABedRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(BookABedRes.ResponseXML, BookABedRes.TransformedResultXML, SearchDetailsList[0]));
        }
    }
}
