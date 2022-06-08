namespace ThirdParty.Service.Tests.Services
{
    using System;
    using ThirdParty.Utility;

    public class Base92ConverterTests
    {
        [Fact]
        public void Encode_Should_ConvertNumberToBase92_When_Called()
        {
            // Arrange
            var converter = new Base92Converter();
            long input = 248276819175;

            // Act
            var encoded = converter.Encode(input);
            var encodedString = new string(encoded);

            //Assert
            Assert.Equal("{Ut[]E   ".Trim(), encodedString.Trim());
        }

        [Fact]
        public void Decode_Should_ConvertBase92ToExpectedDecimal_When_Called()
        {
            // Arrange
            var converter = new Base92Converter();
            string input = "[ici82";

            // Act
            var decoded = converter.Decode(input);

            //Assert
            Assert.Equal(120411435367, decoded);
        }

        [Fact]
        public void Decode_Should_ConvertBase92ToExpectedDecimal_When_WhiteSpacePresent()
        {
            // Arrange
            var converter = new Base92Converter();
            string input = "-c>     ";

            // Act
            var decoded = converter.Decode(input);

            //Assert
            Assert.Equal(260097, decoded);
        }

        [Fact]
        public void Encode_Should_ThrowArguementOutOfRangeException_When_InputTooLarge()
        {
            // Arrange
            var converter = new Base92Converter();
            long input = 5132188731375616;

            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => converter.Encode(input));
        }

        [Fact]
        public void Encode_Should_ThrowArguementOutOfRangeException_When_InputTooSmall()
        {
            // Arrange
            var converter = new Base92Converter();
            long input = -1;

            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => converter.Encode(input));
        }

        [Fact]
        public void Encode_And_Decode_Should_OutputSameValue()
        {
            // Arrange
            var converter = new Base92Converter();
            long input = 5132188731375614;

            // Act
            string encodedToken = new string(converter.Encode(input));
            long decodedToken = converter.Decode(encodedToken);

            // Assert
            Assert.True(input == decodedToken);
        }
    }
}