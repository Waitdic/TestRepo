namespace iVectorOne.Suppliers.TPIntegrationTests.Altura
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Helpers;
    using Intuitive.Helpers.Net;
    using Intuitive.Helpers.Serialization;
    using Moq;
    using iVectorOne.CSSuppliers.Altura;
    using iVectorOne.Lookups;
    using iVectorOne.Search.Models;
    using iVectorOne.Tests.Altura;

    public class AlturaSearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "Altura";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IAlturaSettings _settings = new InjectedAlturaSettings();

        private static readonly ISerializer _serializer = new Serializer();

        private static readonly Mock<ITPSupport> _mockSupport = SetupTPSupportMock();

        public AlturaSearchTest() : base(
            _provider,
            new List<SearchDetails> { _searchDetails },
            _settings,
            new AlturaSearch(_settings, _mockSupport.Object, _serializer))
        {
        }

        private static Mock<ITPSupport> SetupTPSupportMock()
        {
            var mockSupport = new Mock<ITPSupport>();
            mockSupport.Setup(x => x.TPCountryCodeLookupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult("GB"));
            mockSupport.Setup(x => x.TPCurrencyCodeLookupAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult("EUR"));
            mockSupport.Setup(x => x.TPNationalityLookupAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult("GB"));
            return mockSupport;
        }

        public override List<Request> GetRequests(string requestLog)
        {
            var requests = new List<Request>();
            var requestInfo = Helper.LoadRequestLog(requestLog);
            var urls = requestInfo.Item1;
            var requestStrings = requestInfo.Item2;

            SearchDetailsList.Clear();

            for (int i = 0; i < Helper.Rooms.Count; ++i)
            {
                SearchDetailsList.Add(Helper.CreateSearchDetails(Supplier, Helper.Rooms[i]));

                var request = new Request
                {
                    EndPoint = urls[i],
                    Method = RequestMethod.POST,
                    ContentType = ContentTypes.Application_x_www_form_urlencoded,
                    Source = Supplier,
                    Param = "xml",
                    TimeoutInSeconds = 100,
                };

                request.SetRequest(Regex.Replace(requestStrings[i], @"(\r\n\u0020*)(\r*)", string.Empty));

                requests.Add(request);
            }

            return requests;
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(await base.ValidBuildSearchRequestAsync(AlturaRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(AlturaRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(
                base.ValidTransformResponse(
                    AlturaRes.Response,
                    AlturaRes.TransformedResponse,
                    SearchDetailsList[0]));
        }
    }
}