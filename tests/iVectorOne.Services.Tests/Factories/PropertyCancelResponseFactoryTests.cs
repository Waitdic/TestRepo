namespace iVectorOne.Service.Tests.Services
{
    using Moq;
    using iVectorOne.Models;
    using iVectorOne.Factories;
    using iVectorOne.Repositories;
    using iVectorOne.Lookups;

    public class CancelPropertyResponseFactoryTests
    {
        [Fact]
        public void Create_Should_SetTheSupplierCancellationReference_When_ItIsOnTheThirdPartyResponse()
        {
            // Arrange
            var repoMock = new Mock<ITPSupport>();
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
            var repoMock = new Mock<ITPSupport>();
            var factory = new CancelPropertyResponseFactory(repoMock.Object);

            var thirdPartyResponse = new ThirdPartyCancellationResponse();

            // Act
            var response = factory.Create(thirdPartyResponse);

            //Assert
            Assert.Equal("[Failed]", response.SupplierCancellationReference);
        }
    }
}