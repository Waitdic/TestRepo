namespace ThirdParty.Service.Tests.Services
{
    using Moq;
    using ThirdParty.Models;
    using ThirdParty.Factories;
    using ThirdParty.Repositories;

    public class CancelPropertyResponseFactoryTests
    {
        [Fact]
        public void Create_Should_SetTheSupplierCancellationReference_When_ItIsOnTheThirdPartyResponse()
        {
            // Arrange
            var repoMock = new Mock<ICurrencyLookupRepository>();
            var factory = new CancelPropertyResponseFactory(repoMock.Object);

            var thirdPartyResponse = new ThirdPartyCancellationResponse()
            {
                TPCancellationReference = "TPCanxReference",
            };

            // Act
            var response = factory.Create(thirdPartyResponse);

            //Assert
            Assert.Equal("TPCanxReference", response.SupplierCancellationReference);
        }

        [Fact]
        public void Create_Should_SetTheSupplierCancellationReferenceToFailed_When_ItIsNullOrEmptyOnTheThirdPartyResponse()
        {
            // Arrange
            var repoMock = new Mock<ICurrencyLookupRepository>();
            var factory = new CancelPropertyResponseFactory(repoMock.Object);

            var thirdPartyResponse = new ThirdPartyCancellationResponse();

            // Act
            var response = factory.Create(thirdPartyResponse);

            //Assert
            Assert.Equal("[Failed]", response.SupplierCancellationReference);
        }
    }
}