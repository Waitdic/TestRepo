namespace ThirdParty.Suppliers.TPIntegrationTests.iVectorConnect
{
    using System.Collections.Generic;
    using Intuitive.Helpers.Serialization;
    using ThirdParty.Constants;
    using ThirdParty.CSSuppliers.iVectorConnect;
    using ThirdParty.Search.Models;
    using ThirdParty.Tests.iVectorConnect;

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
