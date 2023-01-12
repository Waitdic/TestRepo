namespace iVectorOne.Tests.PremierInn
{
    using Intuitive.Helpers.Net;
    using Intuitive.Helpers.Security;
    using Intuitive.Helpers.Serialization;
    using iVector.Search.Property;
    using iVectorOne.Models.Property;
    using iVectorOne.Search.Models;
    using iVectorOne.Suppliers.PremierInn;
    using iVectorOne.Suppliers.TPIntegrationTests;
    using iVectorOne.Suppliers.TPIntegrationTests.Helpers;
    using System.Text.RegularExpressions;

    public class PremierInnSearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "PremierInn";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IPremierInnSettings _settings = new InjectedPremierInnSettings();

        public PremierInnSearchTest() : base(
            _provider,
            new List<SearchDetails> { _searchDetails },
            _settings,
            new PremierInnSearch(_settings, new Serializer(), new SecretKeeper("testtest")))
        {
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(await base.ValidBuildSearchRequestAsync(PremierInn.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(PremierInn.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(ValidTransformResponse(
                PremierInn.Response,
                PremierInn.TransformedResponse,
                SearchDetailsList[0],
                0,
                Helper.CreateResortSplits(Supplier, new List<Hotel>(), string.Empty)));
        }

        public new bool ValidTransformResponse(string response, string mockResponse, SearchDetails searchDetails, object extraInfo, List<ResortSplit> resortSplits)
        {
            // Arrange 
            var request = CreateResponseWebRequest(response, searchDetails, extraInfo);

            // Act
            var transformedResponse = SearchClass.TransformResponse(new List<Request> { request }, searchDetails, resortSplits);
            var serializer = new Serializer();

            var format = "C SD=\"{0}\"";
            var pattern = string.Format(format, @"(\S+?)");
            var replacement = string.Format(format, string.Empty);

            var innerXml = Regex.Replace(serializer.SerializeWithoutNamespaces(transformedResponse).InnerXml, pattern, replacement);


            // Assert
            return Helper.IsValidResponse(mockResponse, innerXml);
        }
    }
}
