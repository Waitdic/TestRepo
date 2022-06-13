namespace ThirdParty.Suppliers.TPIntegrationTests.DerbySoft
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using ThirdParty.Constants;
    using ThirdParty.CSSuppliers.DerbySoft;
    using ThirdParty.CSSuppliers.DerbySoft.DerbySoftBookingUsbV4;
    using ThirdParty.CSSuppliers.DerbySoft.DerbySoftShoppingEngineV4;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;
    using ThirdParty.Suppliers.TPIntegrationTests.Helpers;
    using ThirdParty.Tests.DerbySoft;

    public class DerbySoftSearchTest : ThirdPartyPropertySearchBaseTest
    {
        private const string _provider = ThirdParties.DERBYSOFTCLUBMED;

        private static readonly SearchDetails _searchDetails = Helper.CreateSearchDetails(_provider);

        private static readonly IDerbySoftSettings _settings = new InjectedDerbySoftSettings();

        public DerbySoftSearchTest() : base(
           _provider,
           new List<SearchDetails>() { _searchDetails },
           _settings,
           new DerbySoftSearch(_settings))
        {
        }

        [Fact]
        public void BuildSearchRequestTest()
        {
            // Arrange 
            var guid = new Guid("2a774834-63a2-4a29-a699-7706e4bb1f42");
            var shoppingEngineRequestBuilder = new ShoppingEngineRequestBuilder(_settings, _provider, guid);
            var bookingEngineRequestBuilder = new BookingUsbV4AvailabilityRequestBuilder(_settings, _provider, guid);
            var shoppingEngineRequests = new List<Request>();
            var bookingusbV4Requests = new List<Request>();

            // Act 
            var shoppingEngineRequstLogs = GetRequests(DerbySoftRes.ShoppingEngineRequestLogs);
            var bookingUsbV4RequestLogs = GetRequests(DerbySoftRes.BookingUsbV4RequestLogs);
            for (int i = 0; i < SearchDetailsList.Count; ++i)
            {
                shoppingEngineRequests.Add(shoppingEngineRequestBuilder.BuildSearchRequests(SearchDetailsList[i], ResortSplits).ToList()[0]);
                bookingusbV4Requests.Add(bookingEngineRequestBuilder.BuildSearchRequests(SearchDetailsList[i], ResortSplits).ToList()[0]);
            }

            // Assert
            Assert.True(Helper.AreSameWebRequests(shoppingEngineRequstLogs, shoppingEngineRequests)); // build request 
            Assert.True(Helper.AreSameWebRequests(bookingUsbV4RequestLogs, bookingusbV4Requests)); // buid request with BookingUsbV4

        }

        [Fact]
        public void TransformerTest()
        {
            // Arrange 
            var shoppingEngineTransformer = new ShoppingEngineResponseTransformer(_settings, _provider);
            var bookingUsbTransformer = new BookingUsbV4ResponseTransformer(_settings, _provider);

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
            var requests = new List<Request> { CreateResponseWebRequest(responseString, SearchDetailsList[0], 1) };
            var searchDetails = SearchDetailsList[0];
            var serializer = new Serializer();

            // Act 
            transformedResults.TransformedResults.AddRange(transformer.TransformResponses(requests, searchDetails));
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