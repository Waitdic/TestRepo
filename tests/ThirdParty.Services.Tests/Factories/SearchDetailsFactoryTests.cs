namespace ThirdParty.API.Tests.Factories
{
    using System;
    using System.Collections.Generic;
    using Moq;
    using ThirdParty.Search.Settings;
    using ThirdParty.SDK.V2.PropertySearch;
    using ThirdParty.Factories;
    using ThirdParty.Models;

    public class SearchDetailsFactoryTests
    {
        [Fact]
        public void Create_Should_SetTheArrivalDate_When_ThereIsAnArrivalDateOnTheRequest()
        {
            // Arrange
            var factory = new SearchDetailsFactory();
            var user = new Subscription();
            var request = new Request()
            {
                ArrivalDate = new DateTime(2000, 01, 01),
            };

            // Act
            var details = factory.Create(request, user, false);

            // Assert
            Assert.Equal(new DateTime(2000, 01, 01), details.ArrivalDate);
        }

        [Fact]
        public void Create_Should_SetTheDuration_When_ThereIsADurationOnTheRequest()
        {
            // Arrange
            var factory = new SearchDetailsFactory();
            var user = new Subscription();
            var request = new Request()
            {
                ArrivalDate = new DateTime(2000, 01, 01),
                Duration = 76,
            };

            // Act
            var details = factory.Create(request, user, false);

            // Assert
            Assert.Equal(76, details.Duration);
        }

        [Fact]
        public void Create_Should_SetDepartureDateToMatch_When_ThereIsADurationAndArrivalDateOnTheRequest()
        {
            // Arrange
            var factory = new SearchDetailsFactory();
            var user = new Subscription();
            var request = new Request()
            {
                ArrivalDate = new DateTime(2000, 01, 01),
                Duration = 10,
            };

            // Act
            var details = factory.Create(request, user, false);

            // Assert
            Assert.Equal(new DateTime(2000, 01, 11), details.DepartureDate);
        }

        [Fact]
        public void Create_Should_SetPropertyReferenceIDs_When_ThereArePropertyIDsOnTheRequest()
        {
            // Arrange
            var factory = new SearchDetailsFactory();
            var user = new Subscription();
            var request = new Request()
            {
                ArrivalDate = new DateTime(2000, 01, 01),
                Properties = new List<int>()
                {
                    1, 3, 5,
                },
            };

            // Act
            var details = factory.Create(request, user, false);

            // Assert
            Assert.Equal(new List<int>() { 1, 3, 5 }, details.PropertyReferenceIDs);
        }

        [Fact]
        public void Create_Should_SetRoomsToCorrectCount_When_ThereAreRoomRequestsOnTheRequest()
        {
            // Arrange
            var factory = new SearchDetailsFactory();
            var user = new Subscription();
            var request = new Request()
            {
                ArrivalDate = new DateTime(2000, 01, 01),
                RoomRequests = new List<RoomRequest>()
                {
                    new RoomRequest(),
                    new RoomRequest(),
                }
            };

            // Act
            var details = factory.Create(request, user, false);

            // Assert
            Assert.Equal(2, details.Rooms);
        }

        [Fact]
        public void Create_Should_SetSettingsToMatch_When_ThereAreSettingsConfiguredAgainstTheUser()
        {
            // Arrange
            var settings = new Settings() { PropertyTPRequestLimit = 52 };
            var factory = new SearchDetailsFactory();
            var user = new Subscription()
            {
                TPSettings = settings,
            };
            var request = new Request()
            {
                ArrivalDate = new DateTime(2000, 01, 01),
            };

            // Act
            var details = factory.Create(request, user, false);

            // Assert
            Assert.Equal(settings, details.Settings);
            Assert.Equal(52, details.Settings.PropertyTPRequestLimit);
        }

        [Fact]
        public void Create_Should_SetCurrencyCodeWhen_ThereIsACurrencyCodeIDOnTheUserSettings()
        {
            // Arrange
            var settings = new Settings() { CurrencyCode = "GBP" };
            var factory = new SearchDetailsFactory();
            var user = new Subscription()
            {
                TPSettings = settings,
            };
            var request = new Request()
            {
                ArrivalDate = new DateTime(2000, 01, 01),
            };

            // Act
            var details = factory.Create(request, user, false);

            // Assert
            Assert.Equal("GBP", details.CurrencyCode);
        }

        [Fact]
        public void Create_Should_SetSetTheTPConfigs_When_TheUserHasThirdPartyConfigs()
        {
            // Arrange
            var configs = new List<ThirdPartyConfiguration>() { new ThirdPartyConfiguration() { Supplier = "Test Supplier" } };
            var factory = new SearchDetailsFactory();
            var user = new Subscription()
            {
                Configurations = configs,
            };
            var request = new Request()
            {
                ArrivalDate = new DateTime(2000, 01, 01),
            };

            // Act
            var details = factory.Create(request, user, false);

            // Assert
            Assert.Equal(configs, details.ThirdPartyConfigurations);
        }

        [Fact]
        public void Create_Should_SetRoomAddultsCorrectly_When_ThereAreRoomRequestsOnTheRequest()
        {
            // Arrange
            var factory = new SearchDetailsFactory();
            var user = new Subscription();
            var request = new Request()
            {
                ArrivalDate = new DateTime(2000, 01, 01),
                RoomRequests = new List<RoomRequest>()
                {
                    new RoomRequest()
                    {
                        Adults = 1
                    },
                    new RoomRequest()
                    {
                        Adults = 5
                    },
                }
            };

            // Act
            var details = factory.Create(request, user, false);

            // Assert
            Assert.Equal(1, details.RoomDetails[0].Adults);
            Assert.Equal(5, details.RoomDetails[1].Adults);
        }

        [Fact]
        public void Create_Should_SetRoomChildrenCorrectly_When_ThereAreRoomRequestsOnTheRequest()
        {
            // Arrange
            var factory = new SearchDetailsFactory();
            var user = new Subscription();
            var request = new Request()
            {
                ArrivalDate = new DateTime(2000, 01, 01),
                RoomRequests = new List<RoomRequest>()
                {
                    new RoomRequest()
                    {
                        Children = 3,
                        ChildAges = new List<int>() { 1, 2, 3 },
                    },
                    new RoomRequest()
                    {
                        Children = 2,
                        ChildAges = new List<int>() { 1, 2 },
                    },
                },
            };

            // Act
            var details = factory.Create(request, user, false);

            // Assert
            Assert.Equal(3, details.RoomDetails[0].Children);
            Assert.Equal(2, details.RoomDetails[1].Children);
        }

        [Fact]
        public void Create_Should_SetRoomInfantsCorrectly_When_ThereAreRoomRequestsOnTheRequest()
        {
            // Arrange
            var factory = new SearchDetailsFactory();
            var user = new Subscription();
            var request = new Request()
            {
                ArrivalDate = new DateTime(2000, 01, 01),
                RoomRequests = new List<RoomRequest>()
                {
                    new RoomRequest()
                    {
                        Infants = 0
                    },
                    new RoomRequest()
                    {
                        Infants = 1
                    },
                },
            };

            // Act
            var details = factory.Create(request, user, false);

            // Assert
            Assert.Equal(0, details.RoomDetails[0].Infants);
            Assert.Equal(1, details.RoomDetails[1].Infants);
        }

        [Fact]
        public void Create_Should_SetRoomChildAgesCorrectly_When_ThereAreRoomRequestsOnTheRequest()
        {

            // Arrange
            var ChildAges1 = new List<int>() { 5, 7 };
            var ChildAges2 = new List<int>();

            var factory = new SearchDetailsFactory();
            var user = new Subscription();
            var request = new Request()
            {
                ArrivalDate = new DateTime(2000, 01, 01),
                RoomRequests = new List<RoomRequest>()
                {
                    new RoomRequest()
                    {
                        Children = ChildAges1.Count,
                        ChildAges = ChildAges1
                    },
                    new RoomRequest()
                    {
                        ChildAges = ChildAges2
                    },
                },
            };

            // Act
            var details = factory.Create(request, user, false);

            // Assert
            Assert.Equal(ChildAges1, details.RoomDetails[0].ChildAges);
            Assert.Equal(ChildAges2, details.RoomDetails[1].ChildAges);
        }

        [Fact]
        public void Create_Should_SetTheLoggingTypeToAll_When_LogIsTrue()
        {
            // Arrange
            var factory = new SearchDetailsFactory();
            var user = new Subscription();
            var request = new Request()
            {
                ArrivalDate = new DateTime(2000, 01, 01),
            };

            // Act
            var details = factory.Create(request, user, true);

            // Assert
            Assert.Equal("All", details.LoggingType);
        }

        [Fact]
        public void Create_Should_SetTheLoggingTypeToNone_When_LogIsFalse()
        {
            // Arrange
            var factory = new SearchDetailsFactory();
            var user = new Subscription();
            var request = new Request()
            {
                ArrivalDate = new DateTime(2000, 01, 01),
            };

            // Act
            var details = factory.Create(request, user, false);

            // Assert
            Assert.Equal("None", details.LoggingType);
        }

        [Fact]
        public void Create_Should_SetTheCurrencyCode_When_ThereIsACurrencyCodeOnTheUserSettings()
        {
            // Arrange
            var settings = new Settings() { CurrencyCode = "GBP" };
            var factory = new SearchDetailsFactory();
            var user = new Subscription()
            {
                TPSettings = settings,
            };
            var request = new Request()
            {
                ArrivalDate = new DateTime(2000, 01, 01),
            };

            // Act
            var details = factory.Create(request, user, false);

            // Assert
            Assert.Equal("GBP", details.CurrencyCode);
        }
    }
}