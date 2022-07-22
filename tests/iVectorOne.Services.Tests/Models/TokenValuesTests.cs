namespace iVectorOne.Service.Tests.Services
{
    using System;
    using iVectorOne.Models.Tokens;
    using iVectorOne.Models.Tokens.Constants;

    public class TokenValuesTests
    {
        [Theory]
        [InlineData(TokenValueType.Adults)]
        [InlineData(TokenValueType.ChildAge7)]
        [InlineData(TokenValueType.PropertyID)]
        public void AddValue_Should_SetTypeCorrectly_When_Called(TokenValueType type)
        {
            // Arrange
            var values = new TokenValues();

            // Act
            values.AddValue(type, 2);

            //Assert
            Assert.Equal(type, values.Values[0].Type);
        }

        [Fact]
        public void AddValue_Should_SetValueCorrectly_When_Called()
        {
            // Arrange
            var values = new TokenValues();

            // Act
            values.AddValue(TokenValueType.CurrencyID, 120);

            //Assert
            Assert.Equal(120, values.Values[0].Value);
        }

        [Fact]
        public void AddValue_Should_AddTheCorrectNumberOfValues_When_Called()
        {
            // Arrange
            var values = new TokenValues();

            // Act
            values.AddValue(TokenValueType.CurrencyID, 2);
            values.AddValue(TokenValueType.PropertyID, 2);
            values.AddValue(TokenValueType.Duration, 2);

            //Assert
            Assert.Equal(3, values.Values.Count);
        }

        [Fact]
        public void AddValue_Should_SetTheBitsCorrectly_When_Called()
        {
            // Arrange
            var values = new TokenValues();

            // Act
            values.AddValue(TokenValueType.PropertyID);
            values.AddValue(TokenValueType.Year);
            values.AddValue(TokenValueType.Month);
            values.AddValue(TokenValueType.Day);
            values.AddValue(TokenValueType.Duration);

            //Assert
            Assert.Equal(TokenValueBits.PropertyIDBits, values.Values[0].Bits);
            Assert.Equal(TokenValueBits.YearBits, values.Values[1].Bits);
            Assert.Equal(TokenValueBits.MonthBits, values.Values[2].Bits);
            Assert.Equal(TokenValueBits.DayBits, values.Values[3].Bits);
            Assert.Equal(TokenValueBits.DurationBits, values.Values[4].Bits);
        }

        [Fact]
        public void AddValue_Should_SetTheStartPositionCorrectly_When_Called()
        {
            // Arrange
            var values = new TokenValues();

            // Act
            values.AddValue(TokenValueType.PropertyID);
            values.AddValue(TokenValueType.Year);
            values.AddValue(TokenValueType.Month);
            values.AddValue(TokenValueType.Day);
            values.AddValue(TokenValueType.Duration);

            //Assert
            Assert.Equal(0, values.Values[0].StartPosition);
            Assert.Equal(TokenValueBits.PropertyIDBits, values.Values[1].StartPosition);
            Assert.Equal(TokenValueBits.PropertyIDBits + TokenValueBits.YearBits, values.Values[2].StartPosition);
            Assert.Equal(TokenValueBits.PropertyIDBits + TokenValueBits.YearBits + TokenValueBits.MonthBits, values.Values[3].StartPosition);
            Assert.Equal(TokenValueBits.PropertyIDBits + TokenValueBits.YearBits + TokenValueBits.MonthBits + TokenValueBits.DayBits, values.Values[4].StartPosition);
        }

        [Fact]
        public void AddValue_Should_ThrowArgumentException_When_ValueOfSameTypeExists()
        {
            // Arrange
            var values = new TokenValues();
            values.AddValue(TokenValueType.Duration);

            // Act, Assert
            Assert.Throws<ArgumentException>("type", () => values.AddValue(TokenValueType.Duration));
        }

        [Fact]
        public void Clear_Should_EmptyValues_When_Called()
        {
            // Arrange
            var values = new TokenValues();

            // Act
            values.AddValue(TokenValueType.PropertyID);
            values.AddValue(TokenValueType.Year);
            values.AddValue(TokenValueType.Month);
            values.AddValue(TokenValueType.Day);
            values.AddValue(TokenValueType.Duration);

            values.Clear();

            //Assert
            Assert.Empty(values.Values);
        }

        [Fact]
        public void GetValue_Should_ReturnExpectedValue_When_Called()
        {
            // Arrange
            var values = new TokenValues();

            // Act
            values.AddValue(TokenValueType.PropertyID, 5076);
            values.AddValue(TokenValueType.Year, 2);
            values.AddValue(TokenValueType.Month, 11);

            //Assert
            Assert.Equal(5076, values.GetValue(TokenValueType.PropertyID));
            Assert.Equal(11, values.GetValue(TokenValueType.Month));
            Assert.Equal(2, values.GetValue(TokenValueType.Year));
        }

        [Fact]
        public void AddValue_Should_ThrowArgumentOutOfRange_When_AValueOverTheNumberOFBitsForATypeIsStored()
        {
            // Arrange
            var values = new TokenValues();

            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => values.AddValue(TokenValueType.Duration, 64));
        }
    }
}