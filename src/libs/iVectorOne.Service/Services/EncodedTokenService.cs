namespace iVectorOne.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using iVectorOne.Models;
    using iVectorOne.Models.Tokens;
    using iVectorOne.Models.Tokens.Constants;
    using iVectorOne.Repositories;
    using iVectorOne.Utility;
    using Microsoft.Extensions.Logging;

    /// <summary>A Service for encrypting and decrypting base 92 tokens.</summary>
    public class EncodedTokenService : ITokenService
    {
        private readonly IPropertyContentRepository _contentRepository;
        private readonly IBaseConverter _converter;
        private readonly ITokenValues _tokenValues;
        private readonly ILogger<EncodedTokenService> _logger;

        /// <summary>Initializes a new instance of the <see cref="EncodedTokenService" /> class.</summary>
        /// <param name="contentRepository">The content repository.</param>
        /// <param name="converter">The base converter.</param>
        /// <param name="tokenValues">The colelction that stores the TokenValues</param>
        /// <param name="logger"></param>
        public EncodedTokenService(
            IPropertyContentRepository contentRepository,
            IBaseConverter converter,
            ITokenValues tokenValues,
            ILogger<EncodedTokenService> logger)
        {
            _contentRepository = Ensure.IsNotNull(contentRepository, nameof(contentRepository));
            _converter = Ensure.IsNotNull(converter, nameof(converter));
            _tokenValues = Ensure.IsNotNull(tokenValues, nameof(tokenValues));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public string EncodeBookingToken(BookToken bookToken)
        {
            _tokenValues.Clear();
            _tokenValues.AddValue(TokenValueType.PropertyID, bookToken.PropertyID);

            char[] bits = ConvertValuesToCharArray(_tokenValues.Values);

            string tokenString = new string(bits.Take(TokenLengths.Book).ToArray());

            return tokenString;
        }

        public string EncodePropertyToken(PropertyToken propertyToken)
        {
            _tokenValues.Clear();
            _tokenValues.AddValue(TokenValueType.PropertyID, propertyToken.PropertyID);
            _tokenValues.AddValue(TokenValueType.Year, propertyToken.ArrivalDate.Year - DateTime.Now.Year);
            _tokenValues.AddValue(TokenValueType.Month, propertyToken.ArrivalDate.Month);
            _tokenValues.AddValue(TokenValueType.Day, propertyToken.ArrivalDate.Day);
            _tokenValues.AddValue(TokenValueType.Duration, propertyToken.Duration);
            _tokenValues.AddValue(TokenValueType.Rooms, propertyToken.Rooms);
            _tokenValues.AddValue(TokenValueType.CurrencyID, propertyToken.CurrencyID);

            char[] bits = ConvertValuesToCharArray(_tokenValues.Values);

            string tokenString = new string(bits.Take(TokenLengths.Property).ToArray());

            return tokenString;
        }

        public string EncodeRoomToken(RoomToken propertyToken)
        {
            _tokenValues.Clear();
            _tokenValues.AddValue(TokenValueType.Adults, propertyToken.Adults);
            _tokenValues.AddValue(TokenValueType.Children, propertyToken.Children);
            _tokenValues.AddValue(TokenValueType.Infants, propertyToken.Infants);
            _tokenValues.AddValue(TokenValueType.PropertyRoomBookingID, propertyToken.PropertyRoomBookingID);

            char[] mainbits = ConvertValuesToCharArray(_tokenValues.Values);

            GenerateChildAgeValues(propertyToken);
            char[] childAgebits = ConvertValuesToCharArray(_tokenValues.Values);

            GenerateMealBasisValues(propertyToken);
            char[] mealBasesBits = ConvertValuesToCharArray(_tokenValues.Values);

            GenerateLocalCostValues(propertyToken);
            char[] localCostBits = ConvertValuesToCharArray(_tokenValues.Values);

            string childAgeToken = new string(childAgebits.Take(TokenLengths.ChildAges).ToArray());
            string roomToken = new string(mainbits.Take(TokenLengths.Room).ToArray());
            string mealbasisToken = new string(mealBasesBits.Take(TokenLengths.MealBasis).ToArray());
            string localCostToken = new string(localCostBits.Take(TokenLengths.LocalCost).ToArray());

            string tokenString = roomToken + childAgeToken + mealbasisToken + localCostToken.TrimEnd();

            return tokenString;
        }

        private char[] ConvertValuesToCharArray(List<TokenValue> values)
        {
            long totalDecimal = 0;

            foreach (var value in values)
            {
                totalDecimal += (long)(value.Value * Math.Pow(2, value.StartPosition));
            }

            char[] bits = _converter.Encode(totalDecimal);

            return bits;
        }

        public async Task<BookToken?> DecodeBookTokenAsync(string tokenString, Account account, string supplierBookingReference)
        {
            BookToken? token = null;
            try
            {
                _tokenValues.Clear();
                _tokenValues.AddValue(TokenValueType.PropertyID);

                GetTokenValues(tokenString);

                token = new BookToken()
                {
                    PropertyID = _tokenValues.GetValue(TokenValueType.PropertyID),
                };

                await PopulateBookTokenFieldsAsync(token, account, supplierBookingReference);
            }
            catch (Exception ex)
            {
                token = null;
                _logger.LogError(ex, "BookTokenDecodeError");
            }

            return token;
        }

        public async Task<PropertyToken?> DecodePropertyTokenAsync(string tokenString, Account account)
        {
            PropertyToken? token = null;

            try
            {
                _tokenValues.Clear();
                _tokenValues.AddValue(TokenValueType.PropertyID);
                _tokenValues.AddValue(TokenValueType.Year);
                _tokenValues.AddValue(TokenValueType.Month);
                _tokenValues.AddValue(TokenValueType.Day);
                _tokenValues.AddValue(TokenValueType.Duration);
                _tokenValues.AddValue(TokenValueType.Rooms);
                _tokenValues.AddValue(TokenValueType.CurrencyID);

                GetTokenValues(tokenString);

                token = new PropertyToken()
                {
                    Duration = _tokenValues.GetValue(TokenValueType.Duration),
                    PropertyID = _tokenValues.GetValue(TokenValueType.PropertyID),
                    Rooms = _tokenValues.GetValue(TokenValueType.Rooms),
                    CurrencyID = _tokenValues.GetValue(TokenValueType.CurrencyID)
                };

                int day = _tokenValues.GetValue(TokenValueType.Day);
                int month = _tokenValues.GetValue(TokenValueType.Month);
                int year = DateTime.Now.AddYears(_tokenValues.GetValue(TokenValueType.Year)).Year;

                if (day > 0 && month > 0)
                {
                    token.ArrivalDate = new DateTime(year, month, day);
                }

                await PopulatePropertyTokenFieldsAsync(token, account);
            }
            catch (Exception ex)
            {
                token = null;
                _logger.LogError(ex, "PropertyTokenDecodeError");
            }

            return token;
        }

        public RoomToken? DecodeRoomToken(string tokenString)
        {
            RoomToken? token = null;

            try
            {
                _tokenValues.Clear();
                _tokenValues.AddValue(TokenValueType.Adults);
                _tokenValues.AddValue(TokenValueType.Children);
                _tokenValues.AddValue(TokenValueType.Infants);
                _tokenValues.AddValue(TokenValueType.PropertyRoomBookingID);

                GetTokenValues(tokenString.Substring(0, 8));

                token = new RoomToken()
                {
                    Adults = _tokenValues.GetValue(TokenValueType.Adults),
                    Children = _tokenValues.GetValue(TokenValueType.Children),
                    Infants = _tokenValues.GetValue(TokenValueType.Infants),
                    PropertyRoomBookingID = _tokenValues.GetValue(TokenValueType.PropertyRoomBookingID)
                };

                _tokenValues.Clear();
                _tokenValues.AddValue(TokenValueType.ChildAge1);
                _tokenValues.AddValue(TokenValueType.ChildAge2);
                _tokenValues.AddValue(TokenValueType.ChildAge3);
                _tokenValues.AddValue(TokenValueType.ChildAge4);
                _tokenValues.AddValue(TokenValueType.ChildAge5);
                _tokenValues.AddValue(TokenValueType.ChildAge6);
                _tokenValues.AddValue(TokenValueType.ChildAge7);
                _tokenValues.AddValue(TokenValueType.ChildAge8);

                GetTokenValues(new string(tokenString.Substring(8, 6).ToArray()));

                token.ChildAges = GetChildAgesFromTokenValues();

                _tokenValues.Clear();

                _tokenValues.AddValue(TokenValueType.MealBasisID3);
                _tokenValues.AddValue(TokenValueType.MealBasisID2);
                _tokenValues.AddValue(TokenValueType.MealBasisID1);

                GetTokenValues(new string(tokenString.Substring(14, 4).ToArray()));

                token.MealBasisID = GetMealBasisFromTokenValues();

                _tokenValues.Clear();

                _tokenValues.AddValue(TokenValueType.LocalCost2);
                _tokenValues.AddValue(TokenValueType.LocalCost1);

                GetTokenValues(new string(tokenString.Skip(18).ToArray()));
                token.LocalCost = GetLocalCostFromTokenValues();
            }
            catch (Exception ex)
            {
                token = null;
                _logger.LogError(ex, "RoomTokenDecodeError");
            }

            return token;
        }

        private async Task PopulatePropertyTokenFieldsAsync(PropertyToken propertyToken, Account account)
        {
            var propertyContent = await _contentRepository.GetContentforPropertyAsync(propertyToken.PropertyID, account, string.Empty);

            if (propertyContent != null)
            {
                propertyToken.Source = propertyContent.Source;
                propertyToken.TPKey = propertyContent.TPKey;
                propertyToken.PropertyName = propertyContent.PropertyName;
                propertyToken.CentralPropertyID = propertyContent.CentralPropertyID;
                propertyToken.GeographyCode = propertyContent.GeographyCode;
                propertyToken.SupplierID = propertyContent.SupplierID;
            }
        }

        private async Task PopulateBookTokenFieldsAsync(BookToken bookToken, Account account, string supplierBookingReference)
        {
            var propertyContent = await _contentRepository.GetContentforPropertyAsync(bookToken.PropertyID, account, supplierBookingReference);

            if (propertyContent != null)
            {
                bookToken.Source = propertyContent.Source;
                bookToken.BookingID = propertyContent.BookingID;
            }
        }

        private void GetTokenValues(string tokenString)
        {
            long decimalValue = _converter.Decode(tokenString);

            foreach (var value in _tokenValues.Values)
            {
                value.Value = (int)((decimalValue >> value.StartPosition) % Math.Pow(2, value.Bits));
            }
        }

        private void GenerateChildAgeValues(RoomToken roomtoken)
        {
            _tokenValues.Clear();

            for (int i = 0; i < 8; i++)
            {
                if (roomtoken.ChildAges.Count > i)
                {
                    Enum.TryParse($"ChildAge{i + 1}", out TokenValueType tokenValueName);

                    _tokenValues.AddValue(tokenValueName, roomtoken.ChildAges[i]);
                }
            }
        }

        private List<int> GetChildAgesFromTokenValues()
        {
            var childAges = new List<int>();

            for (int i = 0; i < 8; i++)
            {
                Enum.TryParse($"ChildAge{i + 1}", out TokenValueType tokenValueName);

                int childAge = _tokenValues.GetValue(tokenValueName);
                if (childAge > 0)
                {
                    childAges.Add(childAge);
                }
            }

            return childAges;
        }

        private void GenerateMealBasisValues(RoomToken roomtoken)
        {
            _tokenValues.Clear();
            int[] mealBases = new int[3];

            if (roomtoken.MealBasisID.Count < 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (i < 3 - roomtoken.MealBasisID.Count)
                    {
                        mealBases[i] = 0;
                    }
                    else
                    {
                        mealBases[i] = roomtoken.MealBasisID[i - 3 + roomtoken.MealBasisID.Count];
                    }
                }
            }
            else
            {
                mealBases = roomtoken.MealBasisID.ToArray();
            }

            for (int i = 3; i > 0; --i)
            {
                Enum.TryParse($"MealBasisID{i}", out TokenValueType tokenValueName);
                _tokenValues.AddValue(tokenValueName, mealBases[i - 1]);
            }
        }

        private List<int> GetMealBasisFromTokenValues()
        {
            var mealbases = new List<int>();

            for (int i = 0; i < 3; ++i)
            {
                Enum.TryParse($"MealBasisID{i + 1}", out TokenValueType tokenValueName);

                int mealbasis = _tokenValues.GetValue(tokenValueName);
                mealbases.Add(mealbasis);
            }

            return mealbases;
        }

        private void GenerateLocalCostValues(RoomToken roomToken)
        {
            _tokenValues.Clear();
            int[] localCosts = new int[2];

            if (roomToken.LocalCost.Count < 2)
            {
                for (int i = 0; i < 2; ++i)
                {
                    if (i < 2 - roomToken.LocalCost.Count)
                    {
                        localCosts[i] = 0;
                    }
                    else
                    {
                        localCosts[i] = roomToken.LocalCost[i - 2 + roomToken.LocalCost.Count];
                    }
                }
            }
            else
            {
                localCosts = roomToken.LocalCost.ToArray();
            }

            for (int i = 2; i > 0; --i)
            {
                Enum.TryParse($"LocalCost{i}", out TokenValueType tokenValueName);
                _tokenValues.AddValue(tokenValueName, localCosts[i - 1]);
            }
        }

        private List<int> GetLocalCostFromTokenValues()
        {
            var localCosts = new List<int>();

            for (int i = 0; i < 2; ++i)
            {
                Enum.TryParse($"LocalCost{i + 1}", out TokenValueType tokenValueName);

                int localCost = _tokenValues.GetValue(tokenValueName);
                localCosts.Add(localCost);
            }

            return localCosts;
        }
    }
}