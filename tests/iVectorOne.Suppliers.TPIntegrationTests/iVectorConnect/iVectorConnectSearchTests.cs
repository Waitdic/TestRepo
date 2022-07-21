namespace iVectorOne.Suppliers.TPIntegrationTests.iVectorConnect
{
    using System.Collections.Generic;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Constants;
    using iVectorOne.CSSuppliers.iVectorConnect;
    using iVectorOne.Search.Models;
    using iVectorOne.Tests.iVectorConnect;

    public class iVectorConnectSearchTests : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = ThirdParties.BOOKABED;

        private static readonly SearchDetails _searchDetails = Helpers.Helper.CreateSearchDetails(_provider);

        private static readonly IiVectorConnectSettings _settings = new InjectediVectorConnectSettings();

        public iVectorConnectSearchTests() : base(
            _provider,
            new List<SearchDetails>() { _searchDetails },
            _settings,
            new iVectorConnectSearch(_settings, new Serializer()))
        {
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(await base.ValidBuildSearchRequestAsync(BookABedRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(BookABedRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(BookABedRes.ResponseXML, BookABedRes.TransformedResultXML, SearchDetailsList[0]));
        }
    }
}
