namespace ThirdParty.Suppliers.TPIntegrationTests.DerbySoft
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive.Net.WebRequests;
    using ThirdParty.Lookups;
    using ThirdParty.Search.Models;
    using ThirdParty.Suppliers.TPIntegrationTests.Helpers;
    using ThirdParty.Search.Support;
    using Xunit;
    using ThirdParty.Results;
    using System.Xml;
    using ThirdParty.CSSuppliers.DerbySoft;
    using ThirdParty.CSSuppliers.DerbySoft.DerbySoftShoppingEngineV4;
    using ThirdParty.Tests.DerbySoft;
    using ThirdParty.CSSuppliers.DerbySoft.DerbySoftBookingUsbV4;
    using Intuitive.Helpers.Serialization;
    using System.Text;

    public class DerbySoftSearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = "DerbySoftClubMed";

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IDerbySoftClubMedSettings _settings = new InjectedDerbySoftClubMedSettings();

        public DerbySoftSearchTest() : base(
           _provider,
           new List<SearchDetails>() { _searchDetails },
           _settings,
           new DerbySoftClubMedSearch(_settings))
        {
        }

        [Fact]
        public void BuildSearchRequestTest()
        {
            // Arrange 
            var guid = new Guid("2a774834-63a2-4a29-a699-7706e4bb1f42");
            var requestBuilderFactory = new SearchFactory(_settings, _provider, guid);
            var shoppingEngineRequestBuilder = new ShoppingEngineRequestBuilder(_settings, _provider, guid);
            var bookingEngineRequestBuilder = new BookingUsbV4AvailabilityRequestBuilder(_settings, _provider, guid);
            var shoppingEngineRequests = new List<Request>();
            var bookingusbV4Requests = new List<Request>();

            // Act 
            var shoppingEngineRequstLogs = GetRequests(DerbySoftRes.ShoppingEngineRequestLogs);
            var bookingUsbV4RequestLogs = GetRequests(DerbySoftRes.BookingUsbV4RequestLogs);
            for (int i = 0; i < SearchDetailsList.Count; ++i)
            {
                shoppingEngineRequests.Add(shoppingEngineRequestBuilder.BuildSearchRequests(SearchDetailsList[i], ResortSplits, true).ToList()[0]);
                bookingusbV4Requests.Add(bookingEngineRequestBuilder.BuildSearchRequests(SearchDetailsList[i], ResortSplits, true).ToList()[0]);
            }

            // Assert
            Assert.True(Helper.AreSameWebRequests(shoppingEngineRequstLogs, shoppingEngineRequests)); // build request 
            Assert.True(Helper.AreSameWebRequests(bookingUsbV4RequestLogs, bookingusbV4Requests)); // buid request with BookingUsbV4

        }

        [Fact]
        public void TransformerTest()
        {
            // Arrange 
            var shoppingEngineTransformer = new ShoppingEngineResponseTransformer(_settings);
            var bookingUsbTransformer = new BookingUsbV4ResponseTransformer(_settings);

            // Act
            var shoopingEngineTransformedResult = GetTransformedResultWithTransformer(shoppingEngineTransformer, DerbySoftRes.ShoppingEngineResponse);
            var bookingEngineTransformedResult = GetTransformedResultWithTransformer(bookingUsbTransformer, DerbySoftRes.BookingEngineResponse);

            // Assert
            Assert.True(Helper.IsValidResponse(DerbySoftRes.BookingEngineTransofrmeResponse, bookingEngineTransformedResult.InnerXml));
            Assert.True(Helper.IsValidResponse(DerbySoftRes.ShoppingEngineTransformedResponse, shoopingEngineTransformedResult.InnerXml));
        }

        private XmlDocument GetTransformedResultWithTransformer(ISearchResponseTransformer transformer, string responseString)
        {
            // Arrange
            var transformedResults = new TransformedResultCollection();
            var guid = new Guid("2a774834-63a2-4a29-a699-7706e4bb1f42");
            var requests = new List<Request> { CreateResponseWebRequest(responseString, SearchDetailsList[0], new SearchExtraHelper { SearchDetails = SearchDetailsList[0], ExtraInfo = "1" }) };
            var searchDetails = SearchDetailsList[0];
            var serializer = new Serializer();

            // Act 
            transformedResults.TransformedResults.AddRange(transformer.TransformResponses(requests));
            var xmlResults = transformedResults.DistinctValidResults.Select(r => serializer.Serialize(r)).ToList();

            var sb = new StringBuilder();
            sb.Append("<Results>");

            foreach (var oXMLDocument in xmlResults)
            {
                if (oXMLDocument is not null)
                {
                    sb.Append(oXMLDocument.InnerXml.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", "").Replace("<?xml version=\"1.0\"?>", "").Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", ""));
                }
            }

            sb.Append("</Results>");

            var oXmlResult = new XmlDocument();

            oXmlResult.LoadXml(sb.ToString());

            return oXmlResult;
        }
    }
}