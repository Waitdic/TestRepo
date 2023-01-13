namespace iVectorOne.Suppliers.TPIntegrationTests.ATI
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Intuitive.Helpers.Net;
    using Intuitive.Helpers.Serialization;
    using iVector.Search.Property;
    using iVectorOne.Search.Models;
    using iVectorOne.Suppliers.ATI;
    using iVectorOne.Suppliers.TPIntegrationTests.Helpers;
    using iVectorOne.Tests.ATI;
    using iVectorOne.Models.Property;
    using Microsoft.Extensions.Caching.Memory;
    using Moq;
    using Xunit;

    public class ATISearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "ATI";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IATISettings _settings = new InjectedATISettings();

        private static readonly Mock<IMemoryCache> _mockCache = SetupMemoryCacheMock();

        public ATISearchTest() : base(
            _provider,
            new List<SearchDetails>() { _searchDetails },
            _settings,
            new ATISearch(_settings, new Serializer(), _mockCache.Object))
        {
        }

        private static Mock<IMemoryCache> SetupMemoryCacheMock()
        {
            var cache = new Mock<IMemoryCache>();

            return cache;
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(await base.ValidBuildSearchRequestAsync(ATIRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(ATIRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(ValidAtiTransformResponse(ATIRes.ResponseXML, ATIRes.TransformedResultXML, SearchDetailsList[0], null!, Helper.CreateResortSplits(Supplier, new List<Hotel>(), string.Empty)));
        }

        private bool ValidAtiTransformResponse(string response, string mockResponse, SearchDetails searchDetails, object extraInfo, List<ResortSplit> resortSplits)
        {
            // Arrange 
            var request = CreateResponseWebRequest(response, searchDetails, extraInfo);

            // Act
            var transformedResponse = SearchClass.TransformResponse(new List<Request> { request }, searchDetails, resortSplits);
            var serializer = new Serializer();
            var transformedResponseXml = serializer.SerializeWithoutNamespaces(transformedResponse).InnerXml;

            string[] formats =
            {
                "SD=\"{0}\"",
                " TPR=\"{0}\""
            };

            var patterns = formats.Select(format => string.Format(format, @"(\S+ ?\S+)")).ToArray();
            var replacements = formats.Select(format => string.Format(format, string.Empty)).ToArray();

            for (int i = 0; i < patterns.Length; i++)
            {
                transformedResponseXml = Regex.Replace(transformedResponseXml, patterns[i], replacements[i]); // remove
            }

            // Assert
            return Helper.IsValidResponse(mockResponse, transformedResponseXml);
        }
    }
}