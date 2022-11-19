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
    }
}
