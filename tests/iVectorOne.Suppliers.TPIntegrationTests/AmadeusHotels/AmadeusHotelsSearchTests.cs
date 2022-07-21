namespace iVectorOne.Suppliers.TPIntegrationTests.AmadeusHotels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Suppliers.AmadeusHotels;
    using Suppliers.AmadeusHotels.Support;
    using Helpers;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using Moq;
    using iVectorOne.Lookups;
    using iVectorOne.Search.Models;
    using Xunit;

    public class AmadeusHotelsSearchTests : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "AmadeusHotels";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IAmadeusHotelsSettings _settings = new InjectedAmadeusHotelsSettings();

        private static readonly Mock<ITPSupport> _mockSupport = SetupTPSupportMock();

        public AmadeusHotelsSearchTests() : base(
            _provider,
            new List<SearchDetails> { _searchDetails },
            _settings,
            new AmadeusHotelsSearch(_settings, new Serializer()))
        {
        }

        private static Mock<ITPSupport> SetupTPSupportMock()
        {
            var mockSupport = new Mock<ITPSupport>();
            return mockSupport;
        }

        public async new Task<bool> ValidBuildSearchRequestAsync(string requestLog)
        {
            // Arrange 
            var mockRequests = GetRequests(requestLog);
            var builtRequests = new List<Request>();
            string[] formats =
            {
                "<add:MessageID xmlns:add=\"http://www.w3.org/2005/08/addressing\">{0}</add:MessageID>",
                "<link:UniqueID>{0}</link:UniqueID>",
                "<oas:Nonce EncodingType=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary\">{0}</oas:Nonce>",
                "<oas:Password Type=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordDigest\">{0}</oas:Password>",
                "<oas1:Created>{0}</oas1:Created>"
            };
            string[] patterns = formats.Select(format => string.Format(format, @"(\S+?)")).ToArray();
            string[] replacements = formats.Select(format => string.Format(format, string.Empty)).ToArray();

            // Act
            foreach (var searchDetails in SearchDetailsList)
            {
                var request = (await SearchClass.BuildSearchRequestsAsync(searchDetails, ResortSplits))[0];
                string requestString = request.RequestString;
                for (int i = 0; i < patterns.Length; i++)
                {
                    requestString = Regex.Replace(requestString, patterns[i], replacements[i]); // remove
                }
               
                request.SetRequest(requestString);
                builtRequests.Add(request);
            }

            // Assert
            return Helper.AreSameWebRequests(mockRequests, builtRequests);
        }

        [Fact]
        public async Task BuiltSearchRequestTest()
        {
            ResortSplits.ForEach(x => x.Hotels.ForEach(r => r.TPKey = $"_{r.TPKey}"));
            
            // Assert 
            Assert.True(await ValidBuildSearchRequestAsync(AmadeusHotelsRes.RequestLog));
            Assert.False(await base.InvalidBuildSearchRequestAsync(AmadeusHotelsRes.RequestLog));
        }

        [Fact]
        public void TransformResponseTest()
        {
            // Assert 
            Assert.True(base.ValidTransformResponse(AmadeusHotelsRes.Response, AmadeusHotelsRes.TransformedResponse, SearchDetailsList[0], new AmadeusSearchHelper()));
        }
    }
}
