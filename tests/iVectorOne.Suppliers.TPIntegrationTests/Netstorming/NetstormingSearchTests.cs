namespace iVectorOne.Suppliers.TPIntegrationTests.WHL
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Suppliers.Netstorming;
    using Helpers;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using Moq;
    using iVectorOne.Lookups;
    using iVectorOne.Search.Models;
    using iVectorOne.Tests.Netstorming;
    using iVectorOne.Models;
    using iVector.Search.Property;

    public class NetstormingSearchTests : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "WHL";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly INetstormingSettings _settings = new InjectedNetstormingSettings();

        private static readonly Mock<ITPSupport> _mockSupport = SetupTPSupportMock();

        public NetstormingSearchTests() : base(
            _provider,
            new List<SearchDetails> { _searchDetails },
            _settings,
            new NetstormingSearch(_settings, _mockSupport.Object, new Serializer()))
        {
        }

        private static Mock<ITPSupport> SetupTPSupportMock()
        {
            var mockSupport = new Mock<ITPSupport>();
            mockSupport.Setup(x => x.TPNationalityLookupAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult("ES"));
            return mockSupport;
        }

        public new async Task<bool> ValidBuildSearchRequestAsync(string requestLog)
        {
            // Arrange 
            var mockRequests = GetRequests(requestLog);
            var builtRequests = new List<Request>();
            const string format = "<timestamp>{0}</timestamp>";
            string pattern = string.Format(format, @"(\d+?)");
            string replacement = string.Format(format, string.Empty);

            // Act
            foreach (var searchDetails in SearchDetailsList)
            {
                var request = (await SearchClass.BuildSearchRequestsAsync(searchDetails, ResortSplits))[0];
                string requestString = Regex.Replace(request.RequestString, pattern, replacement); // remove timestamp
                request.SetRequest(requestString); 
                builtRequests.Add(request);
            }

            foreach (var request in builtRequests)
            {
                request.Source = Supplier;
            }

            // Assert
            return Helper.AreSameWebRequests(mockRequests, builtRequests);
        }

        private bool ValidWhlTransformResponse(string response, string mockResponse, SearchDetails searchDetails, object extraInfo, List<ResortSplit> resortSplits)
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
                " ED=\"{0}\""
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

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(await ValidBuildSearchRequestAsync(NetstormingRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(NetstormingRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(ValidWhlTransformResponse(NetstormingRes.Response, NetstormingRes.TransformedResponse, SearchDetailsList[0], null!, Helper.CreateResortSplits(Supplier, new List<Hotel>(), string.Empty)));
        }
    }
}
