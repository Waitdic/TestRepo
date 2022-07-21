namespace iVectorOne.Suppliers.TPIntegrationTests.Miki
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Intuitive.Helpers.Net;
    using Helpers;
    using Moq;
    using iVectorOne.CSSuppliers.Miki;
    using iVectorOne.Lookups;
    using iVectorOne.Search.Models;
    using Xunit;
    using Intuitive.Helpers.Serialization;

    public class MikiSearchTests : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "Miki";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IMikiSettings _settings = new InjectedMikiSettings();

        private static readonly Mock<ITPSupport> _mockSupport = SetupTPSupportMock();

        public MikiSearchTests() : base(
            _provider,
            new List<SearchDetails> { _searchDetails },
            _settings,
            new MikiSearch(_settings, _mockSupport.Object, new Serializer(), null!))
        {
        }

        private static Mock<ITPSupport> SetupTPSupportMock()
        {
            var mockSupport = new Mock<ITPSupport>();
            mockSupport.Setup(x => x.TPCurrencyCodeLookupAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult("GBP"));
            return mockSupport;
        }

        public async new Task<bool> ValidBuildSearchRequestAsync(string requestLog)
        {
            // Arrange 
            var mockRequests = GetRequests(requestLog);
            var builtRequests = new List<Request>();
            const string format = "<requestID>{0}</requestID><requestDateTime>{1}</requestDateTime>";
            string pattern = string.Format(format, @"(\d+?)", @"(\S+?)");
            string replacement = string.Format(format, string.Empty, string.Empty);

            // Act
            foreach (var searchDetails in SearchDetailsList)
            {
                var request = (await SearchClass.BuildSearchRequestsAsync(searchDetails, ResortSplits))[0];
                string requestString = Regex.Replace(request.RequestString, pattern, replacement); // remove requestID and requestDateTime
                request.SetRequest(requestString);
                builtRequests.Add(request);
            }

            // Assert
            return Helper.AreSameWebRequests(mockRequests, builtRequests);
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(await ValidBuildSearchRequestAsync(MikiRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(MikiRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(
                base.ValidTransformResponse(MikiRes.Response, MikiRes.TransformedResponse, SearchDetailsList[0]));
        }
    }
}
