namespace iVectorOne.Suppliers.TPIntegrationTests.Acerooms
{
    using System.Collections.Generic;
    using Moq;
    using iVectorOne.Lookups;
    using iVectorOne.Search.Models;
    using iVectorOne.Suppliers.TPIntegrationTests.Helpers;
    using iVectorOne.Suppliers.Acerooms;
    using iVectorOne.Tests.Acerooms;
    using iVector.Search.Property;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Models;
    using System.Text.RegularExpressions;
    using Intuitive.Helpers.Net;
    using iVectorOne.Models.Property;

    public class AceroomsSearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "Acerooms";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IAceroomsSettings _settings = new InjectedAceroomsSettings();

        private static readonly Mock<ITPSupport> _mockSupport = SetupTPSupportMock();

        public AceroomsSearchTest() : base(
            _provider,
            new List<SearchDetails>() { _searchDetails },
            _settings,
            new AceroomsSearch(_settings, _mockSupport.Object))
        {
            ResortSplits = Helper.CreateResortSplits(Supplier, Helper.CreateHotels(2795805, 49886, "Torre Hogar", "102245"), "3185");
        }

        private static Mock<ITPSupport> SetupTPSupportMock()
        {
            var mockSupport = new Mock<ITPSupport>();
            mockSupport.Setup(x => x.TPNationalityLookupAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult("115"));
            mockSupport.Setup(x => x.TPCurrencyCodeLookupAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult("GBP"));
            return mockSupport;
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(await base.ValidBuildSearchRequestAsync(AceroomsRes.RequestLogs));
            Assert.False(await base.InvalidBuildSearchRequestAsync(AceroomsRes.RequestLogs));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(ValidTransformResponse(AceroomsRes.Response, AceroomsRes.TransformedResponse, SearchDetailsList[0], null!, Helper.CreateResortSplits(Supplier, new List<Hotel>(), string.Empty)));
        }

        private new bool ValidTransformResponse(string response, string mockResponse, SearchDetails searchDetails, object extraInfo, List<ResortSplit> resortSplits)
        {
            // Arrange 
            var request = CreateResponseWebRequest(response, searchDetails, extraInfo);

            // Act
            var transformedResponse = SearchClass.TransformResponse(new List<Request> { request }, searchDetails, resortSplits);
            var serializer = new Serializer();
            var responseXml = serializer.SerializeWithoutNamespaces(transformedResponse).InnerXml;

            var format = "SD=\"{0}\"";
            var patterns = string.Format(format, @"(\S+?)");
            var replacements = string.Format(format, string.Empty);

            responseXml = Regex.Replace(responseXml, patterns, replacements); // remove
            
            // Assert
            return Helper.IsValidResponse(mockResponse, responseXml);
        }
    }
}