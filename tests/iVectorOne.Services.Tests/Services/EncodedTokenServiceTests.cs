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

    public class EncodedTokenServiceTests
    {
        [Fact]
        public async Task BookEncodeThenDecode_Should_SetTheSameValuesOnABookyToken_When_Called()
        {
            // Arrange
            var tokenService = SetupTokenService(new TokenValues());
            var bookToken = new BookToken()
            {
                PropertyID = 5526100,
            };
            var user = new Account();

            // Act
            var encoded = tokenService.EncodeBookingToken(bookToken);

            var decodedToken = await tokenService.DecodeBookTokenAsync(encoded, user);

            //Assert
            Assert.Equal(bookToken.PropertyID, decodedToken.PropertyID);
        }

        [Fact]
        public async Task PropertyEncodeThenDecode_Should_SetTheSameValuesOnAPropertyToken_When_Called()
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
            var user = new Account();

            // Act
            var encoded = tokenService.EncodePropertyToken(propertyToken);

            var decodedToken = await tokenService.DecodePropertyTokenAsync(encoded, user);

            //Assert
            Assert.Equal(propertyToken.ArrivalDate, decodedToken.ArrivalDate);
            Assert.Equal(propertyToken.Duration, decodedToken.Duration);
            Assert.Equal(propertyToken.PropertyID, decodedToken.PropertyID);
            Assert.Equal(propertyToken.Rooms, decodedToken.Rooms);
            Assert.Equal(propertyToken.ISOCurrencyID, decodedToken.ISOCurrencyID);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task PropertyEncodeThenDecode_Should_HandleDatesForNextTwoYears_When_Called(int yearsAhead)
        {
            // Arrange
            var tokenService = SetupTokenService(new TokenValues());
            var propertyToken = new PropertyToken()
            {
                ArrivalDate = DateTime.Now.Date.AddYears(yearsAhead),
                Duration = 40,
                PropertyID = 5526246
            };
            var user = new Account();

            // Act
            var encoded = tokenService.EncodePropertyToken(propertyToken);
            var decodedToken = await tokenService.DecodePropertyTokenAsync(encoded, user);

            //Assert
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
                MealBasisID = new List<int>() { 89, 34, 56 }
            };

            // Act
            var encoded = tokenService.EncodeRoomToken(roomToken);

            var decodedToken = tokenService.DecodeRoomToken(encoded);

            //Assert
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

            Assert.Equal(89, decodedToken.MealBasisID[0]);
            Assert.Equal(34, decodedToken.MealBasisID[1]);
            Assert.Equal(56, decodedToken.MealBasisID[2]);
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

            var decodedToken = tokenService.DecodeRoomToken(encoded);

            //Assert
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

            var decodedToken = tokenService.DecodeRoomToken(encoded);

            //Assert
            Assert.Equal(roomToken.Adults, decodedToken.Adults);
            Assert.Equal(roomToken.Children, decodedToken.Children);
            Assert.Equal(roomToken.Infants, decodedToken.Infants);
            Assert.Equal(childAges, decodedToken.ChildAges);
        }

        [Fact]
        public async Task DecodePropertyToken_Should_SetValuesFromTheRepository_When_Called()
        {
            // Arrange
            var mockValues = new Mock<ITokenValues>();
            mockValues.SetupGet(r => r.Values).Returns(new List<TokenValue>());
            mockValues.Setup(r => r.GetValue(It.IsAny<TokenValueType>())).Returns(1);
            var tokenService = SetupTokenService(mockValues.Object, 36434, "153535", "ExpediaRapid");
            var user = new Account();

            // Act
            var decodedToken = await tokenService.DecodePropertyTokenAsync("0Pc]I0!", user);

            //Assert
            Assert.Equal(36434, decodedToken.CentralPropertyID);
            Assert.Equal("153535", decodedToken.TPKey);
            Assert.Equal("ExpediaRapid", decodedToken.Source);
        }

        [Fact]
        public async Task DecodeBookToken_Should_SetValuesFromTheRepository_When_Called()
        {
            // Arrange
            var mockValues = new Mock<ITokenValues>();
            mockValues.SetupGet(r => r.Values).Returns(new List<TokenValue>());
            mockValues.Setup(r => r.GetValue(It.IsAny<TokenValueType>())).Returns(1);
            var tokenService = SetupTokenService(mockValues.Object, 36434, "153535", "ExpediaRapid");
            var user = new Account();

            // Act
            var decodedToken = await tokenService.DecodeBookTokenAsync("0Pc]I0!", user);

            //Assert
            Assert.Equal("ExpediaRapid", decodedToken.Source);
        }

        [Theory]
        [InlineData(0, 0, 1)]
        [InlineData(0, 0, 12)]
        [InlineData(0, 2, 23)]
        [InlineData(0, 12, 34)]
        [InlineData(1, 23, 45)]
        [InlineData(12, 34, 56)]
        public void RoomEncodeThenDecode_Should_SetTheMealBasisCorrectly_When_GivenArraysOfVariousLengths(
            int mealBasis1Id,
            int mealBasis2Id,
            int mealBasis3Id)
        {
            // Arrange
            var mealBases = new List<int> { mealBasis1Id, mealBasis2Id, mealBasis3Id };
            var tokenService = SetupTokenService(new TokenValues());
            var roomToken = new RoomToken()
            {
                Adults = 5,
                Children = 4,
                Infants = 7,
                ChildAges = new List<int>(),
                MealBasisID = mealBases,
            };

            // Act
            var encoded = tokenService.EncodeRoomToken(roomToken);

            var decodedToken = tokenService.DecodeRoomToken(encoded);

            //Assert
            Assert.Equal(roomToken.Adults, decodedToken.Adults);
            Assert.Equal(roomToken.Children, decodedToken.Children);
            Assert.Equal(roomToken.Infants, decodedToken.Infants);
            Assert.Equal(mealBases, decodedToken.MealBasisID);
        }

        private EncodedTokenService SetupTokenService(
            ITokenValues tokenValues,
            int centralPropertyId = 0,
            string tpKey = "",
            string source = "")
        {
            var mockRepo = new Mock<IPropertyContentRepository>();

            if (centralPropertyId > 0)
            {
                mockRepo.Setup(r => r.GetContentforPropertyAsync(It.IsAny<int>(), It.IsAny<Account>()))
                .Returns(Task.FromResult(new PropertyContent()
                {
                    CentralPropertyID = centralPropertyId,
                    TPKey = tpKey,
                    Source = source
                }));
            }

            var converter = new Base92Converter();
            var mocklogwriter = new Mock<ILogger<EncodedTokenService>>();
            return new EncodedTokenService(mockRepo.Object, converter, tokenValues, mocklogwriter.Object);
        }
    }
}