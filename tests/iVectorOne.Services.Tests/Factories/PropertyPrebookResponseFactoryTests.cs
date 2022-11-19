namespace iVectorOne.Service.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using Moq;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Factories;
    using iVectorOne.Models.Tokens;
    using iVectorOne.Repositories;
    using iVectorOne.Services;
    using iVectorOne.Lookups;
    using iVectorOne.Models;

    public class PropertyPrebookResponseFactoryTests
    {
        [Fact]
        public async Task Create_Should_PopulateErrataOnTheResponse_When_ErrataExsistsOnThePropertyDetails()
        {
            // Arrange
            var factory = SetupFactory();
            var details = new PropertyDetails()
            {
                Errata = new Errata()
                {
                    new Erratum()
                    {
                        Title = "title1",
                        Text = "text1",
                        Type = "type1"
                    },
                    new Erratum()
                    {
                        Title = "title2",
                        Text = "text2",
                        Type = "type2"
                    }
                }
            };

            // Act
            var response = await factory.CreateAsync(details);

            //Assert
            Assert.Equal(2, response.Errata.Count);
            Assert.Equal("title1: text1" , response.Errata[0]);
            Assert.Equal("title2: text2", response.Errata[1]);
        }

        [Fact]
        public async Task Create_Should_PopulateCancellationsOnTheResponse_When_CancellationsExsistsOnThePropertyDetails()
        {
            // Arrange
            var factory = SetupFactory();
            var details = new PropertyDetails()
            {
                Cancellations = new Cancellations()
                {
                    new Cancellation()
                    {
                        StartDate = DateTime.Now.AddDays(7).Date,
                        EndDate = DateTime.Now.AddDays(14).Date,
                        Amount = 100m
                    },
                    new Cancellation()
                    {
                        StartDate = DateTime.Now.AddDays(14).Date,
                        EndDate = DateTime.Now.AddDays(21).Date,
                        Amount = 200m
                    }
                }
            };

            // Act
            var response = await factory.CreateAsync(details);

            //Assert
            Assert.Equal(2, response.CancellationTerms.Count);
            Assert.Equal(DateTime.Now.AddDays(7).Date, response.CancellationTerms[0].StartDate);
            Assert.Equal(DateTime.Now.AddDays(14).Date, response.CancellationTerms[0].EndDate);
            Assert.Equal(100m, response.CancellationTerms[0].Amount);

            Assert.Equal(DateTime.Now.AddDays(14).Date, response.CancellationTerms[1].StartDate);
            Assert.Equal(DateTime.Now.AddDays(21).Date, response.CancellationTerms[1].EndDate);
            Assert.Equal(200m, response.CancellationTerms[1].Amount);
        }

        [Fact]
        public async Task Create_Should_FormatCancellationAmountCorrect_When_CancellationOnPropertyDetailsLessThan2DP()
        {
            // Arrange
            var factory = SetupFactory();
            var details = new PropertyDetails()
            {
                Cancellations = new Cancellations()
                {
                    new Cancellation()
                    {
                        StartDate = DateTime.Now.AddDays(7).Date,
                        EndDate = DateTime.Now.AddDays(14).Date,
                        Amount = 100.1m
                    },
                }
            };

            // Act
            var response = await factory.CreateAsync(details);

            //Assert
            Assert.Equal("100.10", response.CancellationTerms[0].Amount.ToString());
        }

        [Fact]
        public async Task Create_Should_SetPropertyTokenFromTokenService_When_Called()
        {
            // Arrange
            var factory = SetupFactory();
            var details = new PropertyDetails();

            // Act
            var response = await factory.CreateAsync(details);

            //Assert
            Assert.Equal("TestPropertyToken", response.BookingToken);
        }

        [Fact]
        public async Task Create_Should_PopulateSupplierReferencesFromPropertyDetails_When_Called()
        {
            // Arrange
            var factory = SetupFactory();
            var details = new PropertyDetails() {
                TPRef1 = "ref1",
                TPRef2 = "ref2"
            };

            // Act
            var response = await factory.CreateAsync(details);

            //Assert
            Assert.Equal("ref1", response.SupplierReference1);
            Assert.Equal("ref2", response.SupplierReference2);
        }

        [Fact]
        public async Task Create_Should_PopulateCostFromPropertyDetails_When_Called()
        {
            // Arrange
            var factory = SetupFactory();
            var details = new PropertyDetails()
            {
                LocalCost = 150m,
            };

            // Act
            var response = await factory.CreateAsync(details);

            //Assert
            Assert.Equal(150m, response.TotalCost);
        }

        [Fact]
        public async Task Create_Should_FormatTotalCostCorrectly_When_LessThan2DP()
        {
            // Arrange
            var factory = SetupFactory();
            var details = new PropertyDetails()
            {
                LocalCost = 149.40m,
            };

            // Act
            var response = await factory.CreateAsync(details);

            //Assert
            Assert.Equal("149.40", response.TotalCost.ToString());
        }

        [Fact]
        public async Task Create_Should_PutTheCorrectNumberOfRoomOnTheResponse_When_ThereAreRoomsOnthePropertyDetails()
        {
            // Arrange
            var factory = SetupFactory();
            var details = new PropertyDetails()
            {
                Rooms = new List<RoomDetails>()
                {
                    new RoomDetails(),
                     new RoomDetails()
                }
            };

            // Act
            var response = await factory.CreateAsync(details);

            //Assert
            Assert.Equal(2, response.RoomBookings.Count);
        }

        [Fact]
        public async Task Create_Should_UseTheTokenServiceToBuildARoomToken_When_Called()
        {
            // Arrange
            var factory = SetupFactory();
            var details = new PropertyDetails()
            {
                Rooms = new List<RoomDetails>()
                {
                    new RoomDetails()
                }
            };

            // Act
            var response = await factory.CreateAsync(details);

            //Assert
            Assert.Equal("TestRoomToken", response.RoomBookings[0].RoomBookingToken);
        }

        [Fact]
        public async Task Create_Should_PopulateTheThirdPartyReferenceFromthePropertyDetails_When_Called()
        {
            // Arrange
            var factory = SetupFactory();
            var details = new PropertyDetails()
            {
                Rooms = new List<RoomDetails>()
                {
                    new RoomDetails()
                    {
                        ThirdPartyReference = "TestThirdPartyReference1"
                    }
                }
            };

            // Act
            var response = await factory.CreateAsync(details);

            //Assert
            Assert.Equal("TestThirdPartyReference1", response.RoomBookings[0].SupplierReference);
        }

        [Fact]
        public async Task Create_Should_PopulateTheTotalCostFromthePropertyDetails_When_Called()
        {
            // Arrange
            var factory = SetupFactory();
            var details = new PropertyDetails()
            {
                Rooms = new List<RoomDetails>()
                {
                    new RoomDetails()
                    {
                        LocalCost = 76m
                    }
                }
            };

            // Act
            var response = await factory.CreateAsync(details);

            //Assert
            Assert.Equal(76m, response.RoomBookings[0].TotalCost);
        }

        [Fact]
        public async Task Create_Should_SetTheSupplierFromThePropertyDetails_When_ItsSet()
        {
            // Arrange
            var factory = SetupFactory();
            var details = new PropertyDetails()
            {
                Rooms = new List<RoomDetails>()
                {
                    new RoomDetails()
                },
                Source = "TestSource"
            };

            // Act
            var response = await factory.CreateAsync(details);

            //Assert
            Assert.Equal("TestSource", response.RoomBookings[0].Supplier);
        }

        private PropertyPrebookResponseFactory SetupFactory()
        {
            var tokenServiceMock = new Mock<ITokenService>();
            tokenServiceMock.Setup(ts => ts.EncodePropertyToken(It.IsAny<PropertyToken>())).Returns("TestPropertyToken");
            tokenServiceMock.Setup(ts => ts.EncodeRoomToken(It.IsAny<RoomToken>())).Returns("TestRoomToken");

            var currencyRepoMock = new Mock<ICurrencyLookupRepository>();
            var mealbasisReopoMock = new Mock<IMealBasisLookupRepository>();
            var supportMock = new Mock<ITPSupport>();
            return new PropertyPrebookResponseFactory(
                tokenServiceMock.Object,
                mealbasisReopoMock.Object,
                supportMock.Object);
        }
    }
}