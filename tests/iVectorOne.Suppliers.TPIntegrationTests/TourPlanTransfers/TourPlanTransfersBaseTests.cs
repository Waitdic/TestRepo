namespace iVectorOne.Suppliers.TPIntegrationTests.TourPlanTransfers
{
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Models;
    using iVectorOne.Models.Transfer;
    using iVectorOne.Suppliers.TourPlanTransfers;
    using iVectorOne.Suppliers.TourPlanTransfers.Models;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Moq.Protected;
    using System.Collections;
    using System.Net;

    public class TourPlanTransfersBaseTests
    {
        private readonly ISerializer _serializer;
        private readonly Mock<HttpMessageHandler> mockHttp;

        public TourPlanTransfersBaseTests()
        {
            _serializer = new Serializer();
            mockHttp = new Mock<HttpMessageHandler>();
        }

        [Theory]
        [InlineData(true, "OPT")]
        [InlineData(false, "OPT|OPT-RateId2")]
        [InlineData(false, "OPTRateId1|RateId2")]
        [InlineData(false, "OPT-RateId1|")]
        [InlineData(false, "|OPT-RateId2")]
        public async Task BookAsync_ShouldReturn_Failed_When_Invalid_SupplierReferenceIsPassed(bool oneWay, string supplierReference)
        {
            // Arrange
            TransferDetails transferDetails = new TransferDetails()
            {
                OneWay = oneWay,
                SupplierReference = supplierReference,
            };
            var goWayService = SetupGoWaySydneyTransfersService(transferDetails, new ArrayList());

            // Act
            var bookingStatus = await goWayService.BookAsync(transferDetails);

            //Assert
            mockHttp.Protected().Verify("SendAsync",
            Times.Never(),
            ItExpr.Is<HttpRequestMessage>(req => req.Content.ReadAsStringAsync().Result.Contains("AddServiceRequest")),
            ItExpr.IsAny<CancellationToken>());

            Assert.Equal("failed", bookingStatus);
        }

        [Theory]
        [InlineData("OPT-RateId1|OPT-RateId2", "1234|5678", "testRef1|testRef2", 100)]
        public async Task BookAsync_ShouldReturn_AppropriateResponse_When_Valid_NonOneWay_InputsArePassed(string supplierReference,
           string bookingId, string bookingRef, decimal linePrice)
        {
            // Arrange
            TransferDetails transferDetails = new TransferDetails()
            {
                OneWay = false,
                SupplierReference = supplierReference,
            };

            var bookingIDs = bookingId.Split('|');
            var bookingRefs = bookingRef.Split('|');

            ArrayList serviceReply = new();
            var services = new Services() { Service = new Service() { LinePrice = linePrice } };

            if (!transferDetails.OneWay
                && bookingIDs.Length == 2 && bookingRefs.Length == 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    serviceReply.Add(
                    new AddServiceReply()
                    {
                        BookingId = bookingIDs[i].ToSafeInt(),
                        Ref = bookingRefs[i],
                        Status = "OK",
                        Services = services
                    });
                }
            }

            // Act
            var bookingStatus = GetBookingStatusAsync(transferDetails, serviceReply);

            //Assert
            mockHttp.Protected().Verify("SendAsync",
            Times.Exactly(2),
            ItExpr.Is<HttpRequestMessage>(req => req.Content.ReadAsStringAsync().Result.Contains("AddServiceRequest")),
            ItExpr.IsAny<CancellationToken>());

            Assert.Equal(bookingRef, await bookingStatus);
            Assert.Equal(bookingId, transferDetails.ConfirmationReference);
            Assert.Equal(linePrice * 2, transferDetails.LocalCost);
        }

        [Theory]
        [InlineData("OPT-RateId1", "1234", "testRef1", 100)]
        public async Task BookAsync_ShouldReturn_AppropriateResponse_When_Valid_OneWay_InputsArePassed(string supplierReference,
          string bookingId, string bookingRef, decimal linePrice)
        {
            // Arrange
            TransferDetails transferDetails = new TransferDetails()
            {
                OneWay = true,
                SupplierReference = supplierReference,
            };

            ArrayList serviceReply = new();
            var services = new Services() { Service = new Service() { LinePrice = linePrice } };

            serviceReply.Add(new AddServiceReply()
            {
                BookingId = bookingId.ToSafeInt(),
                Ref = bookingRef,
                Status = "OK",
                Services = services
            });

            // Act
            var bookingStatus = GetBookingStatusAsync(transferDetails, serviceReply);

            //Assert
            mockHttp.Protected().Verify("SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Content.ReadAsStringAsync().Result.Contains("AddServiceRequest")),
            ItExpr.IsAny<CancellationToken>());

            Assert.Equal(bookingRef, await bookingStatus);
            Assert.Equal(bookingId, transferDetails.ConfirmationReference);
            Assert.Equal(linePrice, transferDetails.LocalCost);
        }

        [Theory]
        [InlineData("OPT-RateId1|OPT-RateId2", "1234|5678", "testRef1|testRef2", 100)]
        private async Task BookAsync_ShouldCancel_TheFirstLegBooking_When_SecondLegBooking_Fails(string supplierReference,
           string bookingId, string bookingRef, decimal linePrice)
        {
            // Arrange
            TransferDetails transferDetails = new TransferDetails()
            {
                OneWay = false,
                SupplierReference = supplierReference,
            };

            var bookingIDs = bookingId.Split('|');
            var bookingRefs = bookingRef.Split('|');
            ArrayList serviceReply = new();
            var services = new Services() { Service = new Service() { LinePrice = linePrice } };

            //Arrange FirstLeg Response Success
            serviceReply.Add(
                new AddServiceReply()
                {
                    BookingId = bookingIDs[0].ToSafeInt(),
                    Ref = bookingRefs[0],
                    Status = "OK",
                    Services = services
                });

            //Arrange SecondLeg Response Failure
            serviceReply.Add(
                new AddServiceReply()
                {
                    BookingId = "".ToSafeInt(),
                    Ref = "",
                    Status = "RQ",
                    Services = services
                });

            //Arrange Cancellation Response
            serviceReply.Add(
                new CancelServicesReply
                {
                    ServiceStatuses = new ServiceStatuses()
                    { ServiceStatus = new ServiceStatusContents { Status = "XX" } },
                }
                );

            // Act
            var bookingStatus = GetBookingStatusAsync(transferDetails, serviceReply);

            //Assert
            mockHttp.Protected().Verify("SendAsync",
             Times.Exactly(2),
             ItExpr.Is<HttpRequestMessage>(req => req.Content.ReadAsStringAsync().Result.Contains("AddServiceRequest")),
             ItExpr.IsAny<CancellationToken>());

            mockHttp.Protected().Verify("SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Content.ReadAsStringAsync().Result.Contains("CancelServicesRequest")),
            ItExpr.IsAny<CancellationToken>());

            Assert.Equal("failed", await bookingStatus);
        }


        [Theory]
        [InlineData("SuppRef1", true, "XX")]
        [InlineData("SuppRef2", false, "")]
        [InlineData("SuppRef2|SuppRef3|SuppRef4", false, "")]
        public async Task CancelBookingAsync_ShouldReturn_AppropriateResponse_When_Valid_OneWay_InputsArePassed(string supplierReference,
            bool cancellationSuccess, string responseCancellationStatus)
        {
            // Arrange
            TransferDetails transferDetails = new TransferDetails()
            {
                OneWay = true,
                SupplierReference = supplierReference,
            };

            ArrayList serviceReply = new()
            {
                new CancelServicesReply
                {
                    ServiceStatuses = new ServiceStatuses()
                    { ServiceStatus = new ServiceStatusContents { Status = responseCancellationStatus} },
                }
            };

            // Act
            var cancellationResponse = await GetCancellationStatusAsync(transferDetails, serviceReply);

            //Assert
            mockHttp.Protected().Verify("SendAsync",
            supplierReference.Split('|').Length > 1 ? Times.Never() : Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Content.ReadAsStringAsync().Result.Contains("CancelServicesRequest")),
            ItExpr.IsAny<CancellationToken>());

            Assert.Equal(cancellationSuccess, cancellationResponse.Success);

            Assert.Equal(cancellationSuccess ? supplierReference : "failed", cancellationResponse.TPCancellationReference);
        }

        [Theory]
        [InlineData("SuppRef1|SuppRef2", "XX", "XX", true)]
        [InlineData("SuppRef1|SuppRef2", "XX", "", false)]
        [InlineData("SuppRef1|SuppRef2", "", "XX", false)]
        [InlineData("SuppRef1|SuppRef2", "", "", false)]
        public async Task CancelBookingAsync_ShouldReturn_AppropriateResponse_When_Valid_NonOneWay_InputsArePassed(string supplierReference,
            string responseCancellationStatusLeg1, string responseCancellationStatusLeg2, bool cancellationSuccess)
        {
            // Arrange
            TransferDetails transferDetails = new TransferDetails()
            {
                OneWay = false,
                SupplierReference = supplierReference,
            };

            ArrayList serviceReply = new()
            {
                new CancelServicesReply
                {
                    ServiceStatuses = new ServiceStatuses()
                    { ServiceStatus = new ServiceStatusContents { Status = responseCancellationStatusLeg1} },
                },
                 new CancelServicesReply
                {
                    ServiceStatuses = new ServiceStatuses()
                    { ServiceStatus = new ServiceStatusContents { Status = responseCancellationStatusLeg2} },
                }
            };

            // Act
            var cancellationResponse = await GetCancellationStatusAsync(transferDetails, serviceReply);

            //Assert
            mockHttp.Protected().Verify("SendAsync",
              Times.Exactly(2),
              ItExpr.Is<HttpRequestMessage>(req => req.Content.ReadAsStringAsync().Result.Contains("CancelServicesRequest")),
              ItExpr.IsAny<CancellationToken>());

            Assert.Equal(cancellationSuccess, cancellationResponse.Success);
            Assert.Equal(cancellationSuccess ? supplierReference : "failed", cancellationResponse.TPCancellationReference);
        }

        private async Task<ThirdPartyCancellationResponse> GetCancellationStatusAsync(TransferDetails transferDetails, ArrayList serviceReply)
        {
            var goWayService = SetupGoWaySydneyTransfersService(transferDetails, serviceReply);

            return await goWayService.CancelBookingAsync(transferDetails);
        }

        private async Task<string> GetBookingStatusAsync(TransferDetails transferDetails, ArrayList serviceReply)
        {
            var goWayService = SetupGoWaySydneyTransfersService(transferDetails, serviceReply);

            return await goWayService.BookAsync(transferDetails);
        }


        private TourPlanTransfersBase SetupGoWaySydneyTransfersService(TransferDetails transferDetails,
            ArrayList responseXML)
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
        private HttpClient SetupHttpClient(ArrayList responseXML)
        {
            var response = new List<HttpResponseMessage>();

            foreach (var item in responseXML)
            {
                var xmlResponse = _serializer.SerializeWithoutNamespaces(item);
                xmlResponse.InnerXml = $"<Reply>{xmlResponse.InnerXml}</Reply>";
                response.Add(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(xmlResponse.OuterXml),
                });
            }

            if (responseXML.Count == 4)
            {
                mockHttp
                    .Protected()
                    .SetupSequence<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(response[0])
                   .ReturnsAsync(response[1])
                   .ReturnsAsync(response[2])
                   .ReturnsAsync(response[3]);

            }
            else if (responseXML.Count == 3)
            {
                mockHttp
                    .Protected()
                    .SetupSequence<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(response[0])
                   .ReturnsAsync(response[1])
                   .ReturnsAsync(response[2]);

            }
            else if (responseXML.Count == 2)
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