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
    using MoreLinq;
    using System;
    using System.Collections;
    using System.Net;

    public class TourPlanTransfersBaseTests
    {
        private readonly ISerializer _serializer;
        private readonly Mock<HttpMessageHandler> _mockHttp;

        public TourPlanTransfersBaseTests()
        {
            _serializer = new Serializer();
            _mockHttp = new Mock<HttpMessageHandler>();
        }

        #region BookingTests

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
            goWayService.ValidateSettings(transferDetails);

            // Act
            var bookingStatus = await goWayService.BookAsync(transferDetails);

            //Assert
            _mockHttp.Protected().Verify("SendAsync",
            Times.Never(),
            ItExpr.Is<HttpRequestMessage>(req => req.Content.ReadAsStringAsync().Result.Contains(nameof(AddServiceRequest))),
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
            _mockHttp.Protected().Verify("SendAsync",
            Times.Exactly(2),
            ItExpr.Is<HttpRequestMessage>(req => req.Content.ReadAsStringAsync().Result.Contains(nameof(AddServiceRequest))),
            ItExpr.IsAny<CancellationToken>());

            Assert.Equal(bookingRef, await bookingStatus);
            Assert.Equal(bookingId, transferDetails.ConfirmationReference);
            Assert.Equal(linePrice.DivideBy100M() * 2, transferDetails.LocalCost);
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
            _mockHttp.Protected().Verify("SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Content.ReadAsStringAsync().Result.Contains(nameof(AddServiceRequest))),
            ItExpr.IsAny<CancellationToken>());

            Assert.Equal(bookingRef, await bookingStatus);
            Assert.Equal(bookingId, transferDetails.ConfirmationReference);
            Assert.Equal(linePrice.DivideBy100M(), transferDetails.LocalCost);
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
            _mockHttp.Protected().Verify("SendAsync",
             Times.Exactly(2),
             ItExpr.Is<HttpRequestMessage>(req => req.Content.ReadAsStringAsync().Result.Contains(nameof(AddServiceRequest))),
             ItExpr.IsAny<CancellationToken>());

            _mockHttp.Protected().Verify("SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Content.ReadAsStringAsync().Result.Contains(nameof(CancelServicesRequest))),
            ItExpr.IsAny<CancellationToken>());

            Assert.Equal("failed", await bookingStatus);
        }

        private async Task<string> GetBookingStatusAsync(TransferDetails transferDetails, ArrayList serviceReply)
        {
            var goWayService = SetupGoWaySydneyTransfersService(transferDetails, serviceReply);
            goWayService.ValidateSettings(transferDetails);
            return await goWayService.BookAsync(transferDetails);
        }
        #endregion

        #region CancelBookingTests
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
            _mockHttp.Protected().Verify("SendAsync",
            supplierReference.Split('|').Length > 1 ? Times.Never() : Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Content.ReadAsStringAsync().Result.Contains(nameof(CancelServicesRequest))),
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
            _mockHttp.Protected().Verify("SendAsync",
              Times.Exactly(2),
              ItExpr.Is<HttpRequestMessage>(req => req.Content.ReadAsStringAsync().Result.Contains(nameof(CancelServicesRequest))),
              ItExpr.IsAny<CancellationToken>());

            Assert.Equal(cancellationSuccess, cancellationResponse.Success);
            Assert.Equal(cancellationSuccess ? supplierReference : "failed", cancellationResponse.TPCancellationReference);
        }

        private async Task<ThirdPartyCancellationResponse> GetCancellationStatusAsync(TransferDetails transferDetails, ArrayList serviceReply)
        {
            var goWayService = SetupGoWaySydneyTransfersService(transferDetails, serviceReply);
            goWayService.ValidateSettings(transferDetails);
            
            return await goWayService.CancelBookingAsync(transferDetails);
        }

        #endregion

        #region PrebookTests
        [Theory]
        [InlineData("OPT-RateId1|OPT-RateId2", 1000, "INR")]
        public async Task PreBookAsync_ShouldReturn_AppropriateResponse_When_Valid_NonOneWay_InputsArePassed(string supplierReferences,
           int totalPrice, string currency)
        {
            // Arrange
            TransferDetails transferDetails = new TransferDetails()
            {
                OneWay = false,
                SupplierReference = supplierReferences
            };

            var supplierReference = supplierReferences.Split('|');

            ArrayList serviceReply = new();

            if (!transferDetails.OneWay && supplierReference.Length == 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    serviceReply.Add(generatePrebookResponse(totalPrice, currency, supplierReference, i));
                }
            }

            // Act
            var bookingStatus = await GetPreBookingStatusAsync(transferDetails, serviceReply);

            //Assert
            _mockHttp.Protected().Verify("SendAsync",
            Times.Exactly(2),
            ItExpr.Is<HttpRequestMessage>(req => req.Content.ReadAsStringAsync().Result.Contains(nameof(OptionInfoRequest))),
            ItExpr.IsAny<CancellationToken>());

            Assert.True(bookingStatus);
            Assert.Equal(supplierReferences, transferDetails.SupplierReference);
            Assert.Equal(totalPrice * 2, transferDetails.LocalCost);
            Assert.Equal(currency, transferDetails.ISOCurrencyCode);

            for (int i = 0; i < 2; i++)
            {
                Assert.Contains(i == 0 ? transferDetails.DepartureErrata : transferDetails.ReturnErrata, x => x.Title == $"{i}TestNoteCategory");
                Assert.Contains(i == 0 ? transferDetails.DepartureErrata : transferDetails.ReturnErrata, x => x.Text == $"{i}TestNoteText");
            }

        }

        [Theory]
        [InlineData("OPT-RateId1", 1000, "INR")]
        public async Task PreBookAsync_ShouldReturn_AppropriateResponse_When_Valid_OneWay_InputsArePassed(string supplierReference,
            int totalPrice, string currency)
        {
            // Arrange
            TransferDetails transferDetails = new TransferDetails()
            {
                OneWay = true,
                SupplierReference = supplierReference
            };

            ArrayList serviceReply = new()
            {
                generatePrebookResponse(totalPrice, currency, supplierReference.Split('|'), null)
            };

            // Act
            var bookingStatus = await GetPreBookingStatusAsync(transferDetails, serviceReply);

            //Assert
            _mockHttp.Protected().Verify("SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Content.ReadAsStringAsync().Result.Contains(nameof(OptionInfoRequest))),
            ItExpr.IsAny<CancellationToken>());

            Assert.True(bookingStatus);
            Assert.Equal(supplierReference, transferDetails.SupplierReference);
            Assert.Equal(totalPrice, transferDetails.LocalCost);
            Assert.Equal(currency, transferDetails.ISOCurrencyCode);
            Assert.Contains(transferDetails.DepartureErrata, x => x.Title == "TestNoteCategory");
            Assert.Contains(transferDetails.DepartureErrata, x => x.Text == "TestNoteText");

        }

        [Theory]
        [InlineData(true, "OPT")]
        [InlineData(false, "OPT|OPT-RateId2")]
        [InlineData(false, "OPTRateId1|RateId2")]
        [InlineData(false, "OPT-RateId1|")]
        [InlineData(false, "|OPT-RateId2")]
        public async Task PreBookAsync_ShouldReturn_False_When_Invalid_SupplierReferenceIsPassed(bool oneWay, string supplierReference)
        {
            // Arrange
            TransferDetails transferDetails = new TransferDetails()
            {
                OneWay = oneWay,
                SupplierReference = supplierReference,
            };
            var goWayService = SetupGoWaySydneyTransfersService(transferDetails, new ArrayList());

            // Act
            var preBookingStatus = await goWayService.PreBookAsync(transferDetails);

            //Assert
            _mockHttp.Protected().Verify("SendAsync",
            Times.Never(),
            ItExpr.Is<HttpRequestMessage>(req => req.Content.ReadAsStringAsync().Result.Contains(nameof(OptionInfoRequest))),
            ItExpr.IsAny<CancellationToken>());

            Assert.False(preBookingStatus);
        }

        [Theory]
        [InlineData("OPT-RateId1")]
        public async Task PreBookAsync_ShouldReturn_False_When_Response_HasError_ForOneWay(string supplierReference)
        {
            // Arrange
            TransferDetails transferDetails = new TransferDetails()
            {
                OneWay = true,
                SupplierReference = supplierReference
            };

            ArrayList serviceReply = new()
            {
                new ErrorReply()
                {
                    Error="error"
                }
            };

            // Act
            var bookingStatus = await GetPreBookingStatusAsync(transferDetails, serviceReply);

            //Assert
            _mockHttp.Protected().Verify("SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Content.ReadAsStringAsync().Result.Contains(nameof(OptionInfoRequest))),
            ItExpr.IsAny<CancellationToken>());

            Assert.False(bookingStatus);
        }

        [Theory]
        [InlineData("OPT-RateId1|OPT-RateId2", 1000, "INR")]
        public async Task PreBookAsync_ShouldReturn_False_When_SecondResponse_HasError_ForNonOneWay(string supplierReferences,
            int totalPrice, string currency)
        {
            // Arrange
            TransferDetails transferDetails = new TransferDetails()
            {
                OneWay = false,
                SupplierReference = supplierReferences
            };

            var supplierReference = supplierReferences.Split('|');
            ArrayList serviceReply = new()
            {
                generatePrebookResponse(totalPrice, currency, supplierReference, null),
                new ErrorReply()
                {
                    Error="error"
                }
            };

            // Act
            var bookingStatus = await GetPreBookingStatusAsync(transferDetails, serviceReply);

            //Assert
            _mockHttp.Protected().Verify("SendAsync",
            Times.Exactly(2),
            ItExpr.Is<HttpRequestMessage>(req => req.Content.ReadAsStringAsync().Result.Contains(nameof(OptionInfoRequest))),
            ItExpr.IsAny<CancellationToken>());

            Assert.False(bookingStatus);
        }

        [Theory]
        [InlineData("OPT-RateId1|OPT-RateId2", 1000, "INR")]
        public async Task PreBookAsync_ShouldReturn_False_When_FirstResponse_HasError_ForNonOneWay(string supplierReferences,
            int totalPrice, string currency)
        {
            // Arrange
            TransferDetails transferDetails = new TransferDetails()
            {
                OneWay = false,
                SupplierReference = supplierReferences
            };

            var supplierReference = supplierReferences.Split('|');
            ArrayList serviceReply = new()
            {
                new ErrorReply()
                {
                    Error="error"
                },
                generatePrebookResponse(totalPrice, currency, supplierReference, null)
            };

            // Act
            var bookingStatus = await GetPreBookingStatusAsync(transferDetails, serviceReply);

            //Assert
            _mockHttp.Protected().Verify("SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Content.ReadAsStringAsync().Result.Contains(nameof(OptionInfoRequest))),
            ItExpr.IsAny<CancellationToken>());

            Assert.False(bookingStatus);
        }

        private async Task<bool> GetPreBookingStatusAsync(TransferDetails transferDetails, ArrayList serviceReply)
        {
            var goWayService = SetupGoWaySydneyTransfersService(transferDetails, serviceReply);
            goWayService.ValidateSettings(transferDetails);

            return await goWayService.PreBookAsync(transferDetails);
        }

        private OptionInfoReply generatePrebookResponse(int totalPrice, string currency, string[] supplierReference, int? index)
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
                                    RateId = supplierReference[index??0].Split("-")[1],

                                },
                                OptionNotes = new OptionNotes{
                                    OptionNote=new List<OptionNote>
                                    {
                                        new OptionNote
                                        {
                                        NoteText = index is null ? "TestNoteText" : $"{index}TestNoteText",
                                        NoteCategory= index is null ? "TestNoteCategory" : $"{index}TestNoteCategory"
                                        }
                                    }
                                },
                                Opt=supplierReference[index??0].Split("-")[0],
                            }
                         }
            };
        }

        #endregion

        private TourPlanTransfersBase SetupGoWaySydneyTransfersService(TransferDetails transferDetails,
            ArrayList responseXML)
        {
            transferDetails.ThirdPartySettings = new Dictionary<string, string>
                {
                    { "URL", "https://www.testgoway.com" },
                    { "AgentId", "TestAgentId"},
                    { "Password", "TestPassword" }
                };

            var mockHttpClient = SetupHttpClient(responseXML);

            var mockLogger = new Mock<ILogger<TourPlanTransfersSearchBase>>();

            return new GowaySydneyTransfers.GowaySydneyTransfers(mockHttpClient,
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
                _mockHttp
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
                _mockHttp
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
                _mockHttp
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
                _mockHttp
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(response[0]);
            }

            return new HttpClient(_mockHttp.Object);
        }
    }


}