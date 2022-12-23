namespace iVectorOne.Suppliers.TPIntegrationTests.TourPlanTransfers
{
    using Intuitive.Helpers.Net;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using iVectorOne.Services.Transfer;
    using iVectorOne.Suppliers.TourPlanTransfers;
    using iVectorOne.Suppliers.TourPlanTransfers.Models;
    using iVectorOne.Tests.Helpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System.Collections;
    using System.Collections.Generic;
    using System.Xml;

    public class TourPlanTransfersSearchBaseTests
    {
        private readonly ISerializer _serializer;
        private readonly Mock<HttpMessageHandler> _mockHttp;

        public TourPlanTransfersSearchBaseTests()
        {
            _serializer = new Serializer();
            _mockHttp = new Mock<HttpMessageHandler>();

        }

        [Theory]
        [InlineData(true, "2023-01-03")]
        public async Task BuildSearchRequestsAsync_ShouldReturn_OneRequest_When_Valid_OneWay_InputsArePassed(bool oneWay, DateTime departureDate)
        {
            ArrayList serviceReply = new();
            TransferSearchDetails searchDetails = new()
            {
                OneWay = oneWay,
                DepartureDate = departureDate
            };

            //Act
            var requests = await GetTransferSearchRequestAsync(searchDetails, getLocationMappingMockData(), serviceReply);

            //Assert
            Assert.Single(requests);

        }

        [Theory]
        [InlineData(false, "2025-01-03", "2025-01-10")]
        public async Task BuildSearchRequestsAsync_ShouldReturn_TwoRequests_When_Valid_NotOneWay_InputsArePassed(bool oneWay, DateTime departureDate, DateTime returnDate)
        {
            ArrayList serviceReply = new();
            TransferSearchDetails searchDetails = new()
            {
                OneWay = oneWay,
                DepartureDate = departureDate,
                ReturnDate = returnDate
            };

            //Act
            var requests = await GetTransferSearchRequestAsync(searchDetails, getLocationMappingMockData(), serviceReply);

            //Assert
            Assert.Equal(2, requests.Count);

        }
        [Theory]
        [InlineData(2, 2, 2, "BNE")]
        public async Task BuildSearchRequestsAsync_ShouldReturn_AppropriateResponse_When_Valid_InputsArePassed(int adults, int children, int infants, string locationCode)
        {
            ArrayList serviceReply = new();
            TransferSearchDetails searchDetails = new()
            {
                OneWay = true,
                DepartureDate = DateTime.Now.AddDays(3).Date,
                Adults = adults,
                Children = children,
                Infants = infants,

            };

            //Act
            var requests = await GetTransferSearchRequestAsync(searchDetails, getLocationMappingMockData(), serviceReply);
            var opt = getAttributeValue(requests, "Opt");

            //Assert
            Assert.Equal(searchDetails.Adults, int.Parse(getAttributeValue(requests, "Adults")));
            Assert.Equal(searchDetails.Children, int.Parse(getAttributeValue(requests, "Children")));
            Assert.Equal(searchDetails.Infants, int.Parse(getAttributeValue(requests, "Infants")));
            Assert.Equal(locationCode + "TR????????????", opt);

        }

        [Theory]
        [InlineData(2, 2, 2, "BNE")]
        public async Task BuildSearchRequestsAsync_ShouldReturn_AppropriateRequest_When_NotOneWay(int adults, int children, int infants, string locationCode)
        {
            ArrayList serviceReply = new();
            TransferSearchDetails searchDetails = new()
            {
                OneWay = false,
                DepartureDate = DateTime.Now.AddDays(3).Date,
                ReturnDate = DateTime.Now.AddDays(10).Date,
                Adults = adults,
                Children = children,
                Infants = infants,

            };

            //Act
            var requests = await GetTransferSearchRequestAsync(searchDetails, getLocationMappingMockData(), serviceReply);
            var opt = getAttributeValue(requests, "DateFrom");

            //Assert
            Assert.Equal(searchDetails.DepartureDate.Date, DateTime.Parse(getAttributeValue(requests[0], "DateFrom")));
            Assert.Equal(searchDetails.ReturnDate.Date, DateTime.Parse(getAttributeValue(requests[1], "DateFrom")));


        }

        [Theory]
        [InlineData("BNE: Brisbane Airport", "SYD: Gold Coast Hotel")]
        public async Task BuildSearchRequestsAsync_Check_LocationCode_When_InValid_Locations_ArePassed(string departureLocation, string Returnlocation)
        {
            ArrayList serviceReply = new();
            TransferSearchDetails searchDetails = new()
            {
                OneWay = true,
                DepartureDate = DateTime.Now.AddDays(3).Date,
                ReturnDate = DateTime.Now.AddDays(10).Date,

            };
            LocationMapping location = new LocationMapping()
            {
                DepartureData = departureLocation,
                ArrivalData = Returnlocation
            };

            //Act
            var requests = await GetTransferSearchRequestAsync(searchDetails, location, serviceReply);

            //Assert
            Assert.False(requests.Any());

        }

        [Fact]
        public async Task BuildSearchRequestsAsync_Check_ThirdPartySettings_When_Valid_InputsArePassed()
        {
            Dictionary<string, string> thirdPartySettings = new Dictionary<string, string>
                {
                    { "URL", "https://www.testgoway.com" },
                    { "AgentId", "TestAgentId"},
                    { "Password", "TestPassword" }
                };
            ArrayList serviceReply = new();
            TransferSearchDetails searchDetails = new()
            {
                OneWay = false,
                DepartureDate = DateTime.Now.AddDays(3).Date
            };

            //Act
            var requests = await GetTransferSearchRequestAsync(searchDetails, getLocationMappingMockData(), serviceReply);

            //Assert
            Assert.Equal(thirdPartySettings, searchDetails.ThirdPartySettings);

        }

        [Fact]
        public async Task BuildSearchRequestsAsync_Check_ThirdPartySettings_When_InValid_InputsArePassed()
        {
            Dictionary<string, string> thirdPartySettings = new Dictionary<string, string>
                {
                    { "URL", "https://www.testgoway.com" },
                    { "AgentId", "TestAgentId22"},
                    { "Password", "TestPassword11" }
                };
            ArrayList serviceReply = new();
            TransferSearchDetails searchDetails = new()
            {
                OneWay = false,
                DepartureDate = DateTime.Now.AddDays(3).Date
            };

            //Act
            var requests = await GetTransferSearchRequestAsync(searchDetails, getLocationMappingMockData(), serviceReply);

            //Assert
            Assert.NotEqual(thirdPartySettings, searchDetails.ThirdPartySettings);

        }
        [Theory]
        [InlineData("SYD:DepartureData1", "SYD:ArrivalData1", "DepartureData1 to ArrivalData1", "SYDTR????????????", "comment")]
        public Task TransformResponse_ShouldReturn_AppropriateResponse_When_Valid_OneWay_InputsArePassed(string departureData, string arrivalData, string description, string opt, string comment)
        {
            // Arrange
            LocationMapping locationMapping = new()
            {
                ArrivalData = arrivalData,
                DepartureData = departureData
            };
            OptionInfoReply optionInfoReply = GenerateResponse(100, "INR", opt, description, comment);
            List<Request> requests = new();
            Request request = new()
            {
                ExtraInfo = Constant.Outbound,
            };
            request.SetResponse(Serialize(optionInfoReply).OuterXml);
            requests.Add(request);
            // Act
            var transformResponse = GetTransformResponse(new TransferSearchDetails(), locationMapping, requests, new());
            var option = optionInfoReply.Option.FirstOrDefault();

            Assert.Equal(option.OptStayResults.TotalPrice, transformResponse.TransformedResults[0].Cost);
            Assert.Equal(option.OptStayResults.Currency, transformResponse.TransformedResults[0].CurrencyCode);
            Assert.Equal(option.OptStayResults.TotalPrice, transformResponse.TransformedResults[0].Cost);
            Assert.Equal(option.Opt + "-" + option.OptStayResults.RateId, transformResponse.TransformedResults[0].SupplierReference);
            return Task.CompletedTask;
        }

        [Theory]
        [InlineData("transfervehicle1", "transfervehicle3", "SYD-Default|SYD1-Default")]
        public Task TransformResponse_ShouldNotReturn_Matching_Transfer_Vehicle_When_InValid_InputsArePassed(string commentDeparture, string commentArrival, string supplierReference)
        {

            List<Request> requests = new();
            Request request1 = new()
            {
                ExtraInfo = Constant.Outbound,
            };
            Request request2 = new();
            request1.SetResponse(Serialize(GenerateResponse(100, "INR", "SYD", "Brisbane Airport to Gold Coast Hotel", commentDeparture)).OuterXml);
            request2.SetResponse(Serialize(GenerateResponse(200, "INR", "SYD1", "Gold City Hotel to Brisbane Airport", commentArrival)).OuterXml);

            requests.Add(request1);
            requests.Add(request2);

            // Act
            var transformResponse = GetTransformResponse(new TransferSearchDetails(), getLocationMappingMockData(), requests, new());

            //Assert
            Assert.False(transformResponse.TransformedResults.Any());

            return Task.CompletedTask;
        }

        [Theory]
        [InlineData("transfervehicle1", "transfervehicle1", "SYD-Default|SYD1-Default")]
        public Task TransformResponse_ShouldReturn_Matching_Transfer_Vehicle_When_Valid_TwoWay_InputsArePassed(string commentDeparture, string commentArrival, string supplierReference)
        {

            List<Request> requests = new();
            Request request1 = new()
            {
                ExtraInfo = Constant.Outbound,
            };
            Request request2 = new();
            request1.SetResponse(Serialize(GenerateResponse(100, "INR", "SYD", "Brisbane Airport to Gold Coast Hotel", commentDeparture)).OuterXml);
            request2.SetResponse(Serialize(GenerateResponse(200, "INR", "SYD1", "Gold Coast Hotel to Brisbane Airport", commentArrival)).OuterXml);

            requests.Add(request1);
            requests.Add(request2);

            // Act
            var transformResponse = GetTransformResponse(new TransferSearchDetails(), getLocationMappingMockData(), requests, new());
            var result = transformResponse.TransformedResults.FirstOrDefault();

            Assert.Equal(result.Cost, 300);
            Assert.Equal(result.SupplierReference, supplierReference);
            Assert.True(transformResponse.TransformedResults.Any());
            return Task.CompletedTask;
        }

        [Theory]
        [InlineData("transfervehicle1", "transfervehicle1", "SYD-Default|SYD1-Default")]
        public Task TransformResponse_ShouldReturn_Matching_Transfer_Vehicle_When_InValid_TwoWay_InputsArePassed(string commentDeparture, string commentArrival, string supplierReference)
        {
            List<Request> requests = new();
            Request request1 = new()
            {
                ExtraInfo = Constant.Outbound,
            };
            Request request2 = new();
            request1.SetResponse(Serialize(GenerateResponse(100, "INR", "SYD", "Brisbane Airport to Gold Coast Hotel", commentDeparture)).OuterXml);
            request2.SetResponse(Serialize(GenerateResponse(200, "INR", "SYD1", "City Hotel to Brisbane Airport", commentArrival)).OuterXml);
            requests.Add(request1);
            requests.Add(request2);
            // Act
            var transformResponse = GetTransformResponse(new TransferSearchDetails(), getLocationMappingMockData(), requests, new());

            Assert.False(transformResponse.TransformedResults.Any());
            return Task.CompletedTask;
        }

        [Theory]
        [InlineData(false)]
        public Task TransformResponse_ShouldReturn_Only_FreesaleResults_When_IncludeOnRequest_IsFalse(bool includeOnRequest)
        {

            List<Request> requests = new();
            Request request1 = new()
            {
                ExtraInfo = Constant.Outbound,
            };
            Request request2 = new();
            TransferSearchDetails transferSearchDetails = new TransferSearchDetails()
            {
                IncludeOnRequest = includeOnRequest,
            };
            request1.SetResponse(Serialize(GenerateResponse(100, "INR", "SYD", "Brisbane Airport to Gold Coast Hotel", "transfervehicle1")).OuterXml);
            request2.SetResponse(Serialize(GenerateResponse(200, "INR", "SYD", "Gold Coast Hotel to Brisbane Airport", "transfervehicle1")).OuterXml);

            requests.Add(request1);
            requests.Add(request2);

            // Act
            var transformResponse = GetTransformResponse(transferSearchDetails, getLocationMappingMockData(), requests, new());
            var result = transformResponse.TransformedResults.FirstOrDefault();

            Assert.False(result.OnRequest);
            return Task.CompletedTask;
        }

        [Theory]
        [InlineData(true)]
        public Task TransformResponse_ShouldReturn_FreesaleAndOnRequestResults_When_IncludeOnRequest_IsTrue(bool includeOnRequest)
        {

            List<Request> requests = new();
            Request request1 = new()
            {
                ExtraInfo = Constant.Outbound,
            };
            Request request2 = new();
            TransferSearchDetails transferSearchDetails = new TransferSearchDetails()
            {
                IncludeOnRequest = includeOnRequest,
            };
            request1.SetResponse(Serialize(GenerateResponse(100, "INR", "SYD", "Brisbane Airport to Gold Coast Hotel", "transfervehicle1")).OuterXml);
            request2.SetResponse(Serialize(GenerateResponse(200, "INR", "SYD", "Gold Coast Hotel to Brisbane Airport", "transfervehicle1", "RQ")).OuterXml);

            requests.Add(request1);
            requests.Add(request2);

            // Act
            var transformResponse = GetTransformResponse(transferSearchDetails, getLocationMappingMockData(), requests, new());
            var result = transformResponse.TransformedResults.FirstOrDefault();

            Assert.True(result.OnRequest);
            return Task.CompletedTask;
        }
        private string getAttributeValue(List<Request> requests, string tagName)
            => requests.FirstOrDefault().RequestXML.GetElementsByTagName(tagName)[0].InnerText;

        private TransformedTransferResultCollection GetTransformResponse(TransferSearchDetails searchDetails, LocationMapping location, List<Request> requests = null, ArrayList serviceReply = null)
        {
            var goWaySearchService = SetupGoWaySydneyTransfersService(searchDetails, serviceReply);

            return goWaySearchService.TransformResponse(requests, searchDetails, location);
        }


        private XmlDocument Serialize(object obj)
        {
            var xmlResponse = _serializer.SerializeWithoutNamespaces(obj);
            xmlResponse.InnerXml = $"<Reply>{xmlResponse.InnerXml}</Reply>";
            return xmlResponse;
        }
        private LocationMapping getLocationMappingMockData()
        {
            return new()
            {
                DepartureData = "BNE: Brisbane Airport",
                ArrivalData = "BNE: Gold Coast Hotel"
            };
        }

        private async Task<List<Request>> GetTransferSearchRequestAsync(TransferSearchDetails searchDetails, LocationMapping locationMapping,
            ArrayList serviceReply)
        {
            var goWayService = SetupGoWaySydneyTransfersService(searchDetails, serviceReply);

            return await goWayService.BuildSearchRequestsAsync(searchDetails, locationMapping);
        }
        private string getAttributeValue(Request requests, string tagName)
           => requests.RequestXML.GetElementsByTagName(tagName)[0].InnerText;
        private OptionInfoReply GenerateResponse(int totalPrice, string currency, string opt, string description, string comment, string availability = "OK")
        {
            return new OptionInfoReply()
            {
                Option = new List<Option>
                     {
                     new Option()
                     {
                     OptStayResults= new OptStayResults()
                     {
                     TotalPrice = totalPrice,
                     Currency=currency,
                     RateId = "Default",
                     Availability= availability

                     },
                     OptionNotes = new OptionNotes{
                     OptionNote=new List<OptionNote>() }
                    ,
                     Opt= opt,
                     OptGeneral = new OptGeneral()
                     {
                         Description = description,
                         Comment = comment
                     }
                     }
                     }
            };
        }
        private TourPlanTransfersSearchBase SetupGoWaySydneyTransfersService(TransferSearchDetails searchDetails,
        ArrayList responseXML)
        {
            searchDetails.ThirdPartySettings = new Dictionary<string, string>
                {
                    { "URL", "https://www.testgoway.com" },
                    { "AgentId", "TestAgentId"},
                    { "Password", "TestPassword" }
                };
            var mockHttpClient = new HttpClient(_mockHttp.Object);
            var mocklocationManagerService = new Mock<ILocationManagerService>();
            var mockLogger = new Mock<ILogger<TourPlanTransfersSearchBase>>();


            return new GowaySydneyTransfers.GowaySydneyTransfersSearch(mockHttpClient,
               _serializer, mockLogger.Object, mocklocationManagerService.Object);
        }
    }
}

