namespace ThirdParty.Suppliers.TPIntegrationTests.Stuba
{
    using System.Collections.Generic;
    using Intuitive.Helpers.Serialization;
    using Moq;
    using ThirdParty.CSSuppliers.Stuba;
    using ThirdParty.Lookups;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Support;
    using ThirdParty.Suppliers.TPIntegrationTests.Helpers;
    using ThirdParty.Tests.Stuba;

    public class StubaSearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "Stuba";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IStubaSettings _settings = new InjectedStubaSettings();

        public StubaSearchTest() : base(
            _provider,
            new List<SearchDetails>() { _searchDetails },
            _settings,
            new StubaSearch(_settings, new Mock<ITPSupport>().Object, new Serializer()))
        {
        }

        [Fact]
        public void BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(base.ValidBuildSearchRequest(StubaRes.RequestLog));
            Assert.False(base.InvalidBuildSearchRequest(StubaRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(StubaRes.ResponseString, StubaRes.TransformedResultXML, SearchDetailsList[0]));
        }
    }
}