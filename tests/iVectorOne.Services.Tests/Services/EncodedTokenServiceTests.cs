namespace iVectorOne.Service.Tests.Services
{
    using Microsoft.Extensions.Logging;
    using Moq;
    using System;
    using System.Collections.Generic;
    using iVectorOne.Models;
    using iVectorOne.Models.Tokens;
    using iVectorOne.Repositories;
    using iVectorOne.Services;
    using iVectorOne.Utility;
    using iVectorOne.Models.Property;
    using iVectorOne.Lookups;

    public class EncodedTokenServiceTests
    {
        [Fact]
        public void BookEncodeThenDecode_Should_SetTheSameValuesOnABookyToken_When_Called()
        {
            // Arrange
            var tokenService = SetupTokenService(new TokenValues());
            var bookToken = new BookToken()
            {
                PropertyID = 5526100,
            };

            // Act
            var encoded = tokenService.EncodeBookingToken(bookToken);

            var decodedToken = tokenService.DecodeBookToken(encoded)!;

            //Assert
            Assert.NotNull(decodedToken);
            Assert.Equal(bookToken.PropertyID, decodedToken.PropertyID);
        }

        [Fact]
        public void PropertyEncodeThenDecode_Should_SetTheSameValuesOnAPropertyToken_When_Called()
        {
            // Arrange
            var tokenService = SetupTokenService(new TokenValues());
            var propertyToken = new PropertyToken()
            {
                ArrivalDate = DateTime.Now.Date,
                Duration = 40,
                PropertyID = 5526246,
                Rooms = 3,
                ISOCurrencyID = 52
            };

            // Act
            var encoded = tokenService.EncodePropertyToken(propertyToken);

            var decodedToken = tokenService.DecodePropertyToken(encoded)!;

            //Assert
            Assert.NotNull(decodedToken);
            Assert.Equal(propertyToken.ArrivalDate, decodedToken.ArrivalDate);
            Assert.Equal(propertyToken.Duration, decodedToken.Duration);
            Assert.Equal(propertyToken.PropertyID, decodedToken.PropertyID);
            Assert.Equal(propertyToken.Rooms, decodedToken.Rooms);
            Assert.Equal(propertyToken.ISOCurrencyID, decodedToken.ISOCurrencyID);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void PropertyEncodeThenDecode_Should_HandleDatesForNextTwoYears_When_Called(int yearsAhead)
        {
            // Arrange
            var tokenService = SetupTokenService(new TokenValues());
            var propertyToken = new PropertyToken()
            {
                ArrivalDate = DateTime.Now.Date.AddYears(yearsAhead),
                Duration = 40,
                PropertyID = 5526246
            };

            // Act
            var encoded = tokenService.EncodePropertyToken(propertyToken);
            var decodedToken = tokenService.DecodePropertyToken(encoded)!;

            //Assert
            Assert.NotNull(decodedToken);
            Assert.Equal(propertyToken.ArrivalDate, decodedToken.ArrivalDate);
        }

        [Fact]
        public void RoomEncodeThenDecode_Should_SetTheSameValuesOnARoomToken_When_Called()
        {
            // Arrange
            var tokenService = SetupTokenService(new TokenValues());
            var roomToken = new RoomToken()
            {
                Adults = 2,
                Children = 8,
                Infants = 4,
                ChildAges = new List<int>() { 3, 4, 4, 4, 4, 9, 7, 16 },
                MealBasisID = 5
            };

            // Act
            var encoded = tokenService.EncodeRoomToken(roomToken);

            var decodedToken = tokenService.DecodeRoomToken(encoded)!;

            //Assert
            Assert.NotNull(decodedToken);
            Assert.Equal(roomToken.Adults, decodedToken.Adults);
            Assert.Equal(roomToken.Children, decodedToken.Children);
            Assert.Equal(roomToken.Infants, decodedToken.Infants);

            Assert.Equal(3, decodedToken.ChildAges[0]);
            Assert.Equal(4, decodedToken.ChildAges[1]);
            Assert.Equal(4, decodedToken.ChildAges[2]);
            Assert.Equal(4, decodedToken.ChildAges[3]);
            Assert.Equal(4, decodedToken.ChildAges[4]);
            Assert.Equal(9, decodedToken.ChildAges[5]);
            Assert.Equal(7, decodedToken.ChildAges[6]);
            Assert.Equal(16, decodedToken.ChildAges[7]);

            Assert.Equal(5, decodedToken.MealBasisID);
        }

        [Fact]
        public void RoomEncodeThenDecode_Should_NotSetChildAges_When_ThereArentAny()
        {
            // Arrange
            var tokenService = SetupTokenService(new TokenValues());
            var roomToken = new RoomToken()
            {
                Adults = 15,
                Children = 0,
                Infants = 1,
                ChildAges = new List<int>() { }
            };

            // Act
            var encoded = tokenService.EncodeRoomToken(roomToken);

            var decodedToken = tokenService.DecodeRoomToken(encoded)!;

            //Assert
            Assert.NotNull(decodedToken);
            Assert.Equal(roomToken.Adults, decodedToken.Adults);
            Assert.Equal(roomToken.Children, decodedToken.Children);
            Assert.Equal(roomToken.Infants, decodedToken.Infants);
            Assert.Empty(decodedToken.ChildAges);
        }

        [Fact]
        public void RoomEncodeThenDecode_Should_SetTheChildAgesCorrectly_When_GivenArraysOfVariousLengths()
        {
            // Arrange
            var tokenService = SetupTokenService(new TokenValues());
            var childAges = new List<int>() { 17, 2, 15, 13 };
            var roomToken = new RoomToken()
            {
                Adults = 5,
                Children = 4,
                Infants = 7,
                ChildAges = childAges,
            };

            // Act
            var encoded = tokenService.EncodeRoomToken(roomToken);

            var decodedToken = tokenService.DecodeRoomToken(encoded)!;

            //Assert
            Assert.NotNull(decodedToken);
            Assert.Equal(roomToken.Adults, decodedToken.Adults);
            Assert.Equal(roomToken.Children, decodedToken.Children);
            Assert.Equal(roomToken.Infants, decodedToken.Infants);
            Assert.Equal(childAges, decodedToken.ChildAges);
        }

        private EncodedTokenService SetupTokenService(ITokenValues tokenValues)
        {
            var converter = new Base92Converter();
            var mocklogwriter = new Mock<ILogger<EncodedTokenService>>();
            return new EncodedTokenService(converter, tokenValues, mocklogwriter.Object);
        }
    }
}