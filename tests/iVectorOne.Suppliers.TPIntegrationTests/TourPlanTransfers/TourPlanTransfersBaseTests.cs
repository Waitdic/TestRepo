namespace iVectorOne.Suppliers.TPIntegrationTests.TourPlanTransfers
{
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Models.Transfer;
    using iVectorOne.Suppliers.TourPlanTransfers;
    using iVectorOne.Suppliers.TourPlanTransfers.Models;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Moq.Protected;
    using System.Net;

    public class TourPlanTransfersBaseTests
    {
        private readonly ISerializer _serializer;

        public TourPlanTransfersBaseTests()
        {
            _serializer = new Serializer();
        }

        [Theory]
        [InlineData(true, "OPT", "failed")]
        [InlineData(false, "OPT|OPT-RateId2", "failed")]
        [InlineData(false, "OPTRateId1|RateId2", "failed")]
        [InlineData(false, "OPT-RateId1|", "failed")]
        [InlineData(false, "|OPT-RateId2", "failed")]
        public async Task BookAsync_ShouldReturn_Failed_When_Invalid_SupplierReferenceIsPassed(bool oneWay, string supplierReference,
            string bookingStatusToAssert)
        {
            // Arrange
            TransferDetails transferDetails = new TransferDetails()
            {
                OneWay = oneWay,
                SupplierReference = supplierReference,
            };
            var goWayService = SetupGoWaySydneyTransfersService(transferDetails, new List<AddServiceReply>());

            // Act
            var bookingStatus = await goWayService.BookAsync(transferDetails);

            //Assert
            Assert.Equal(bookingStatusToAssert, bookingStatus);
        }

        [Theory]
        [InlineData(true, "OPT-RateId1", "1234", "testRef1", 100)]
        [InlineData(false, "OPT-RateId1|OPT-RateId2", "1234|5678", "testRef1|testRef2", 100)]
        public async Task BookAsync_ShouldReturn_AppropriateResponse_When_Valid_InputsArePassed(bool oneWay, string supplierReference,
           string bookingId, string bookingRef, decimal linePrice)
        {
            // Arrange
            TransferDetails transferDetails = new TransferDetails()
            {
                OneWay = oneWay,
                SupplierReference = supplierReference,
            };

            var bookingIDs = bookingId.Split('|');
            var bookingRefs = bookingRef.Split('|');
            List<AddServiceReply> serviceReply = new();
            var services = new Services() { Service = new Service() };
            services.Service.LinePrice = linePrice;

            if (transferDetails.OneWay
                && bookingIDs.Length == 1 && bookingRefs.Length == 1)
            {
                serviceReply.Add(new AddServiceReply()
                {
                    BookingId = bookingId.ToSafeInt(),
                    Ref = bookingRef,
                    Status = "OK",
                    Services = services
                });
            }
            else if (!transferDetails.OneWay
                && bookingIDs.Length == 2 && bookingRefs.Length == 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    serviceReply.Add(
                    new AddServiceReply()
                    {
                        BookingId = bookingId.Split('|')[i].ToSafeInt(),
                        Ref = bookingRef.Split('|')[i],
                        Status = "OK",
                        Services = services
                    });
                }
            }


            // Act
            var bookingStatus = GetBookingStatusAsync(transferDetails, serviceReply);

            //Assert
            Assert.Equal(bookingRef, await bookingStatus);
            Assert.Equal(bookingId, transferDetails.ConfirmationReference);
            Assert.Equal(transferDetails.OneWay ? linePrice : linePrice * 2, transferDetails.LocalCost);
        }

        private async Task<string> GetBookingStatusAsync<T>(TransferDetails transferDetails, List<T> serviceReply) where T : class
        {
            var goWayService = SetupGoWaySydneyTransfersService(transferDetails, serviceReply);

            return await goWayService.BookAsync(transferDetails);
        }


        private TourPlanTransfersBase SetupGoWaySydneyTransfersService<T>(TransferDetails transferDetails,
            List<T> responseXML) where T : class
        {
            var mockITourPlanTransfersSettings = new Mock<ITourPlanTransfersSettings>();
            var mockHttpClient = SetupHttpClient(responseXML);

            var mockLogger = new Mock<ILogger<TourPlanTransfersSearchBase>>();

            mockITourPlanTransfersSettings.Setup(x => x.URL(transferDetails))
                .Returns("https://www.testgoway.com");

            mockITourPlanTransfersSettings.Setup(x => x.AgentId(transferDetails))
                .Returns("testAgentId");

            mockITourPlanTransfersSettings.Setup(x => x.Password(transferDetails))
                .Returns("testPassword");

            return new GowaySydneyTransfers.GowaySydneyTransfers(mockITourPlanTransfersSettings.Object, mockHttpClient,
               mockLogger.Object, _serializer);
        }
        private HttpClient SetupHttpClient<T>(List<T> responseXML) where T : class
        {
            var response = new List<HttpResponseMessage>();

            foreach (var item in responseXML)
            {
                var xmlRequest = _serializer.SerializeWithoutNamespaces(item);
                xmlRequest.InnerXml = $"<Reply>{xmlRequest.InnerXml}</Reply>";
                response.Add(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(xmlRequest.OuterXml),
                });
            }

            var mockHttp = new Mock<HttpMessageHandler>();

            if (responseXML.Count == 2)
            {
                mockHttp
                    .Protected()
                    .SetupSequence<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(response[0])
                   .ReturnsAsync(response[1]);

            }
            else if (responseXML.Count == 1)
            {
                mockHttp
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(response[0]);
            }

            return new HttpClient(mockHttp.Object);
        }
    }


}