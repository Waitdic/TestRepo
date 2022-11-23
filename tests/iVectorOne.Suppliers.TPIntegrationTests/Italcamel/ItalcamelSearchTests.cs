namespace iVectorOne.Tests.Italcamel
{
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Search.Models;
    using iVectorOne.Suppliers.Italcamel;
    using iVectorOne.Suppliers.TPIntegrationTests;
    using iVectorOne.Suppliers.TPIntegrationTests.Helpers;

    public class ItalcamelSearchTests : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "Italcamel";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IItalcamelSettings _settings = new InjectedItalcamelSettings();

        public ItalcamelSearchTests() 
            : base(_provider,
                new List<SearchDetails> { _searchDetails },
                _settings,
                new ItalcamelSearch(_settings, new Serializer()))
        {
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            Assert.True(await base.ValidBuildSearchRequestAsync(ItalcamelRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(ItalcamelRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(ItalcamelRes.Response, ItalcamelRes.TransformedResponse, SearchDetailsList[0]));
        }
    }
}
