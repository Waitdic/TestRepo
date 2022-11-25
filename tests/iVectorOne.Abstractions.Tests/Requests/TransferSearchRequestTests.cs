namespace iVectorOne.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using iVectorOne.SDK.V2;
    using iVectorOne.SDK.V2.TransferSearch;

    public class TransferRequestTests
    {
        [Fact]
        public void Validate_Should_AddCorrectWarning_When_DepartureDateIsNull()
        {
            //Arrange
            var request = new Request();
            var validator = new Validator();

            //Act
            var warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.DepartureDateNotSpecified, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_DepartureLocationIDIsRequired()
        {
            //Arrange
            Request request = new Request();
            Validator validator = new Validator();

            //Act
            FluentValidation.Results.ValidationResult warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.InvalidDepartureLocationID, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_ArrivalLocationIDIsRequired()
        {
            //Arrange
            Request request = new Request();
            Validator validator = new Validator();

            //Act
            FluentValidation.Results.ValidationResult warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.InvalidArrivalLocationID, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_DepartureDateIsInThePast()
        {
            //Arrange
            Request request = new Request()
            {
                DepartureDate = DateTime.Now.AddDays(-1)
            };
            Validator validator = new Validator();

            //Act
            FluentValidation.Results.ValidationResult warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.DepartureDateInThePast, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_DepartureDateIsTooFarInFuture()
        {
            //Arrange
            Request request = new Request()
            {
                DepartureDate = DateTime.Now.AddYears(3)
            };
            Validator validator = new Validator();

            //Act
            FluentValidation.Results.ValidationResult warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.DepartureDateToFarInTheFuture, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_OneWayIsFalse_ReturnDateSpecified()
        {
            //Arrange
            Request request = new Request()
            {
                OneWay = false
            };
            Validator validator = new Validator();

            //Act
            FluentValidation.Results.ValidationResult warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.ReturnDateSpecified, warnings.Errors.Select(e => e.ErrorMessage));
        }
        [Fact]
        public void Validate_Should_AddCorrectWarning_When_OneWayIsTrue_ReturnDateNotSpecified()
        {
            //Arrange
            Request request = new Request()
            {
                OneWay = true,
                ReturnDate =DateTime.Now.Date
                
            };
            Validator validator = new Validator();

            //Act
            FluentValidation.Results.ValidationResult warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.ReturnDateNotSpecified, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_InvalidSupplier()
        {
            //Arrange
            Request request = new Request();
            
            Validator validator = new Validator();

            //Act
            FluentValidation.Results.ValidationResult warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.InvalidSupplier, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_Only15AdultsAllowedTransfer()
        {
            //Arrange
            var request = new Request()
            {
               Adults =16
            };
            var validator = new Validator();

            //Act
            var warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.Only15AdultsAllowedTransfer, warnings.Errors.Select(e => e.ErrorMessage));
        }


        [Fact]
        public void Validate_Should_AddCorrectWarning_When_Only8ChildrenAllowedTransfer()
        {
            //Arrange
            var request = new Request()
            {
                Children = 9
            };

            var validator = new Validator();

            //Act
            var warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.Only8ChildrenAllowedTransfer, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_Only7InfantsAllowedTransfer()
        {
            //Arrange
            var request = new Request()
            {  
                Infants = 8
            };
            var validator = new Validator();

            //Act
            var warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.Only7InfantsAllowedTransfer, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_NoAdultsOrChildrenSpecified()
        {
            //Arrange
            var request = new Request();
           
            var validator = new Validator();

            //Act
            var warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.NoAdultsOrChildrenSpecified, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_OneWayIsFalse_ReturnTimeSpecified()
        {
            //Arrange
            Request request = new Request()
            {
                OneWay = false,
                ReturnTime = string.Empty
            };

            Validator validator = new Validator();

            //Act
            FluentValidation.Results.ValidationResult warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.ReturnTimeSpecified, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_OneWayIsTrue_ReturnTimeNotSpecified()
        {
            //Arrange
            Request request = new Request()
            {
                OneWay = true,
                ReturnTime = "10:22"
            };

            Validator validator = new Validator();

            //Act
            FluentValidation.Results.ValidationResult warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.ReturnTimeNotSpecified, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_OneWayIsTrue_ReturnTimeInvalid()
        {
            //Arrange
            Request request = new Request()
            {
                OneWay = false,
                ReturnTime = "10:61"
            };

            Validator validator = new Validator();

            //Act
            FluentValidation.Results.ValidationResult warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.ReturnTimeInvalid, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_OneWayIsTrue_ReturnTimeIsvalid()
        {
            //Arrange
            Request request = new Request()
            {
                OneWay = false,
                ReturnTime = "10:21"
            };

            Validator validator = new Validator();

            //Act
            FluentValidation.Results.ValidationResult warnings = validator.Validate(request);

            // Assert
            Assert.NotSame(WarningMessages.ReturnTimeInvalid, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_DepartureTimeInvalid()
        {
            //Arrange
            Request request = new Request()
            {
                DepartureTime = "10:61"
            };

            Validator validator = new Validator();

            //Act
            FluentValidation.Results.ValidationResult warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.DepartureTimeInvalid, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_DepartureTimeIsvalid()
        {
            //Arrange
            Request request = new Request()
            {
                DepartureTime = "10:11"
            };

            Validator validator = new Validator();

            //Act
            FluentValidation.Results.ValidationResult warnings = validator.Validate(request);

            // Assert
            Assert.NotSame(WarningMessages.DepartureTimeInvalid, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_AddCorrectWarning_When_OneWayIsTrue_DepartureTimeRequired()
        {
            //Arrange
            Request request = new Request()
            {
                DepartureTime = string.Empty
            };

            Validator validator = new Validator();

            //Act
            FluentValidation.Results.ValidationResult warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.DepartureTimeRequired, warnings.Errors.Select(e => e.ErrorMessage));
        }


        [Fact]
        public void Validate_Should_AddCorrectWarning_When_OneWayIsTrue_InvalidDuration()
        {
            //Arrange
            Request request = new Request()
            {
                DepartureDate = DateTime.Now.Date,
                ReturnDate = DateTime.Now.AddDays(65).Date
            };

            Validator validator = new Validator();

            //Act
            FluentValidation.Results.ValidationResult warnings = validator.Validate(request);

            // Assert
            Assert.Contains(WarningMessages.InvalidDuration, warnings.Errors.Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Validate_Should_NoWarning_When_Request_All_Field_Valid()
        {
            //Arrange
            Request request = new Request()
            {
                DepartureLocationID = 1,
                ArrivalLocationID = 1,
                DepartureDate = DateTime.Today.AddDays(2),
                DepartureTime = "10:00",
                OneWay = false,
                ReturnDate = DateTime.Today.AddDays(8).Date,
                ReturnTime = "10:22" ,
                Supplier = "nulltesttransfersupplier",
                Adults = 2,
                Children = 1,
                Infants = 0,
                ChildAges = new List<int>() { 1,2,3,4,5,6,7 }
            };

            Validator validator = new Validator();

            //Act
            FluentValidation.Results.ValidationResult warnings = validator.Validate(request);

            // Assert
            Assert.False(warnings.Errors.Any());
        }
    }
}
