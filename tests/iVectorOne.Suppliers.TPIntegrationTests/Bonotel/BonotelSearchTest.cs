namespace iVectorOne.Suppliers.TPIntegrationTests.Bonotel
{
    using System.Collections.Generic;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.CSSuppliers.Bonotel;
    using iVectorOne.Search.Models;
    using iVectorOne.Suppliers.TPIntegrationTests.Helpers;
    using iVectorOne.Tests.Bonotel;

    public class BonotelSearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "Bonotel";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IBonotelSettings _settings = new InjectedBonotelSettings();

        public BonotelSearchTest() : base(
            _provider,
            new List<SearchDetails>() { _searchDetails },
            _settings,
            new BonotelSearch(_settings, new Serializer()))
        {
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(await base.ValidBuildSearchRequestAsync(BonotelRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(BonotelRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(BonotelRes.ResponseXML, BonotelRes.TransformedResultXML, SearchDetailsList[0]));
        }
    }
}