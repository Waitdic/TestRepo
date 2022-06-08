﻿namespace ThirdParty.Suppliers.TPIntegrationTests.Altura
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Helpers;
    using Intuitive.Net.WebRequests;
    using Intuitive.Helpers.Serialization;
    using Moq;
    using ThirdParty.CSSuppliers.Altura;
    using ThirdParty.Lookups;
    using ThirdParty.Search.Models;
    using ThirdParty.Tests.Altura;

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
            mockSupport.Setup(x => x.TPCountryCodeLookup(It.IsAny<string>(), It.IsAny<string>())).Returns("GB");
            mockSupport.Setup(x => x.TPCurrencyLookup(It.IsAny<string>(), It.IsAny<string>())).Returns("EUR");
            mockSupport.Setup(x => x.TPNationalityLookup(It.IsAny<string>(), It.IsAny<string>())).Returns("GB");
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
                    Method = eRequestMethod.POST,
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
        public void BuiltSearchRequestTest()
        {
            // Assert 
            Assert.True(base.ValidBuildSearchRequest(AlturaRes.RequestLog));
            Assert.False(base.InvalidBuildSearchRequest(AlturaRes.RequestLog));
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