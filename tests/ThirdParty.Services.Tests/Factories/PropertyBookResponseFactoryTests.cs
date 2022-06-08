namespace ThirdParty.Service.Tests.Services
{
    using Moq;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.Factories;
    using ThirdParty.Models.Tokens;
    using ThirdParty.Repositories;
    using ThirdParty.Services;

    public class PropertyBookResponseFactoryTests
    {
        [Fact]
        public void Create_Should_BuildTheResponseCorrectly_When_Called()
        {
            // Arrange
            var tokenServiceMock = new Mock<ITokenService>();
            tokenServiceMock.Setup(ts => ts.EncodeBookToken(It.IsAny<BookToken>())).Returns("TestToken");

            var currencyRepoMock = new Mock<ICurrencyLookupRepository>();
            var factory = new PropertyBookResponseFactory(tokenServiceMock.Object, currencyRepoMock.Object);

            var details = new PropertyDetails() {
                CurrencyID = 5,
                TPPropertyID = 5,
                SupplierSourceReference = "testREf",
                SourceSecondaryReference = "TestsecondaryRef",
                TPRef1 = "testTpREf1"
            };

            // Act
            var response = factory.Create(details);

            //Assert
            Assert.Equal("testREf", response.SupplierBookingReference);
            Assert.Equal("TestToken", response.BookToken);
            Assert.Equal("TestsecondaryRef", response.SupplierReference1);
            Assert.Equal("testTpREf1", response.SupplierReference2);
        }
    }
}