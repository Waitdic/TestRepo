namespace ThirdParty.Suppliers.TPIntegrationTests.Restel
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Helpers;
    using Intuitive.Helpers.Security;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using ThirdParty.CSSuppliers.Restel;
    using ThirdParty.Search.Models;
    using ThirdParty.Tests.Restel;

    public class RestelSearchTests : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "Restel";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IRestelSettings _settings = new InjectedRestelSettings();

        public RestelSearchTests() : base(
            _provider,
            new List<SearchDetails> { _searchDetails },
            _settings,
            new RestelSearch(_settings, new Serializer(), new SecretKeeper("testtest")))
        {
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
            Assert.True(base.ValidBuildSearchRequest(RestelRes.RequestLog));
            Assert.False(base.InvalidBuildSearchRequest(RestelRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(RestelRes.ResponseXML, RestelRes.TransformedResultXML, SearchDetailsList[0]));
        }
    }
}