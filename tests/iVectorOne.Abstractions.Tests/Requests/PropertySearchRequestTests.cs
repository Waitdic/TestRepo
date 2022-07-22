namespace iVectorOne.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using iVectorOne.SDK.V2;
    using iVectorOne.SDK.V2.PropertySearch;

    public class RequestTests
    {
        [Fact]
        public void Validate_Should_AddCorrectWarning_When_ArrivalDateIsNull()
        {
            //Arrange
            var request = new Request();
            var validator = new Validator();

            //Act
            var warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.ArrivalDateNotSpecified, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_ArrivalDateIsInThePast()
        {
            //Arrange
            var request = new Request
            {
                ArrivalDate = DateTime.Now.AddDays(-1)
            };
            var validator = new Validator();

            //Act
            var warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.ArrivalDateInThePast, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_ArrivalDateIsTooFarInFuture()
        {
            //Arrange
            var request = new Request
            {
                ArrivalDate = DateTime.Now.AddYears(3)
            };
            var validator = new Validator();

            //Act
            var warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.ArrivalDateToFarInTheFuture, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_DurationNotSpecified()
        {
            //Arrange
            var request = new Request()
            {
                Duration = -1,
            };
            var validator = new Validator();

            //Act
            var warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.DurationNotSpecified, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_PropertiesNotSpecified()
        {
            //Arrange
            var request = new Request();
            var validator = new Validator();

            //Act
            var warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.PropertyNotSpecified, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_MoreThan500PropertiesProvided()
        {
            //Arrange
            var request = new Request();
            var validator = new Validator();

            for (int i = 0; i < 502; i++)
            {
                request.Properties.Add(i);
            }

            //Act
            var warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.PropertiesOverLimit, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_RoomsNotSpecified()
        {
            //Arrange
            var request = new Request();
            var validator = new Validator();

            //Act
            var warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.RoomsNotSpecified, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_RoomsAdultsNotInAllRooms()
        {
            //Arrange
            var request = new Request()
            {
                RoomRequests = new List<RoomRequest>()
                {
                    new RoomRequest()
                    {
                        Adults = 1
                    },
                    new RoomRequest()
                    {
                        Adults = 0
                    },
                }
            };
            var validator = new Validator();

            //Act
            var warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.AdultsNotSpecifiedInAllRooms, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_MoreThan15AdultsInARoom()
        {
            //Arrange
            var request = new Request()
            {
                RoomRequests = new List<RoomRequest>()
                {
                    new RoomRequest()
                    {
                        Adults = 16
                    },
                    new RoomRequest()
                    {
                        Adults = 2
                    },
                }
            };
            var validator = new Validator();

            //Act
            var warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.Only15AdultsAllowed, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_MoreThan8ChildrenInARoom()
        {
            //Arrange
            var request = new Request()
            {
                RoomRequests = new List<RoomRequest>()
                {
                    new RoomRequest()
                    {
                        Adults = 2,
                        Children = 9
                    },
                    new RoomRequest()
                    {
                        Adults = 2
                    },
                }
            };
            var validator = new Validator();

            //Act
            var warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.Only8ChildrenAllowed, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_MoreThan7InfantsInARoom()
        {
            //Arrange
            var request = new Request()
            {
                RoomRequests = new List<RoomRequest>()
                {
                    new RoomRequest()
                    {
                        Adults = 2,
                        Infants = 8
                    },
                    new RoomRequest()
                    {
                        Adults = 2
                    },
                }
            };
            var validator = new Validator();

            //Act
            var warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.Only7InfantsAllowed, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_TheChildrenInAroomDontMatchTheChildAges()
        {
            //Arrange
            var request = new Request()
            {
                RoomRequests = new List<RoomRequest>()
                {
                    new RoomRequest()
                    {
                        Adults = 2,
                        Children = 3,
                        ChildAges = new List<int>(){ 4, 7 }
                    }
                }
            };
            var validator = new Validator();

            //Act
            var warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.ChildAgesDontMatchChildren, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_ChildAgesAreOver17()
        {
            //Arrange
            var request = new Request()
            {
                RoomRequests = new List<RoomRequest>()
                {
                    new RoomRequest()
                    {
                        Adults = 2,
                        Children = 2,
                        ChildAges = new List<int>(){ 4, 18 }
                    }
                }
            };
            var validator = new Validator();

            //Act
            var warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.InvalidChildAge, warnings.Errors.Select(e => e.ErrorMessage));
        }
    }
}
