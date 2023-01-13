namespace iVectorOne.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive;
    using iVectorOne.Models.Tokens;
    using iVectorOne.Models.Tokens.Constants;
    using iVectorOne.Utility;
    using Microsoft.Extensions.Logging;
    using System.Globalization;
    using iVectorOne.Models.Tokens.Transfer;
    using iVectorOne.Models.Tokens.Extra;

    /// <summary>A Service for encrypting and decrypting base 92 tokens.</summary>
    public class EncodedTokenService : ITokenService
    {
        private readonly IBaseConverter _converter;
        private readonly ITokenValues _tokenValues;
        private readonly ILogger<EncodedTokenService> _logger;

        /// <summary>Initializes a new instance of the <see cref="EncodedTokenService" /> class.</summary>
        /// <param name="converter">The base converter.</param>
        /// <param name="tokenValues">The colelction that stores the TokenValues</param>
        /// <param name="logger"></param>
        public EncodedTokenService(
            IBaseConverter converter,
            ITokenValues tokenValues,
            ILogger<EncodedTokenService> logger)
        {
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
            _tokenValues.AddValue(TokenValueType.CurrencyID, propertyToken.ISOCurrencyID);

            char[] bits = ConvertValuesToCharArray(_tokenValues.Values);

            string tokenString = new(bits.Take(TokenLengths.Property).ToArray());

            return tokenString;
        }

        public string EncodeRoomToken(RoomToken propertyToken)
        {
            _tokenValues.Clear();
            _tokenValues.AddValue(TokenValueType.PropertyID, propertyToken.PropertyID);
            _tokenValues.AddValue(TokenValueType.MealBasisID, propertyToken.MealBasisID);
            _tokenValues.AddValue(TokenValueType.Adults, propertyToken.Adults);
            _tokenValues.AddValue(TokenValueType.Children, propertyToken.Children);
            _tokenValues.AddValue(TokenValueType.Infants, propertyToken.Infants);
            _tokenValues.AddValue(TokenValueType.PropertyRoomBookingID, propertyToken.PropertyRoomBookingID);

            char[] mainbits = ConvertValuesToCharArray(_tokenValues.Values);

            GenerateChildAgeValues(propertyToken);
            char[] childAgebits = ConvertValuesToCharArray(_tokenValues.Values);

            GenerateLocalCostValues(propertyToken);
            char[] localCostBits = ConvertValuesToCharArray(_tokenValues.Values);

            string childAgeToken = new(childAgebits.Take(TokenLengths.ChildAges).ToArray());
            string roomToken = new(mainbits.Take(TokenLengths.Room).ToArray());
            string localCostToken = new(localCostBits.Take(TokenLengths.LocalCost).ToArray());

            string tokenString = roomToken + childAgeToken + localCostToken.TrimEnd();

            return tokenString;
        }

        public string EncodeTransferToken(TransferToken transferToken)
        {
            (int, int) departureTime = GetTimeParts(transferToken.DepartureTime);
            (int, int) returnTime = transferToken.OneWay ? (0,0) : GetTimeParts(transferToken.ReturnTime);
                
            _tokenValues.Clear();
            _tokenValues.AddValue(TokenValueType.Year, transferToken.DepartureDate.Year - DateTime.Now.Year);
            _tokenValues.AddValue(TokenValueType.Month, transferToken.DepartureDate.Month);
            _tokenValues.AddValue(TokenValueType.Day, transferToken.DepartureDate.Day);
            _tokenValues.AddValue(TokenValueType.Hour1, departureTime.Item1);
            _tokenValues.AddValue(TokenValueType.Minute1, departureTime.Item2);
            _tokenValues.AddValue(TokenValueType.Duration, transferToken.Duration);
            _tokenValues.AddValue(TokenValueType.OneWay, transferToken.OneWay ? 1 : 0);
            _tokenValues.AddValue(TokenValueType.Hour2, returnTime.Item1);
            _tokenValues.AddValue(TokenValueType.Minute2, returnTime.Item2);

            char[] timeBits = ConvertValuesToCharArray(_tokenValues.Values);

            _tokenValues.Clear();
            _tokenValues.AddValue(TokenValueType.CurrencyID, transferToken.ISOCurrencyID);
            _tokenValues.AddValue(TokenValueType.Adults, transferToken.Adults);
            _tokenValues.AddValue(TokenValueType.Children, transferToken.Children);
            _tokenValues.AddValue(TokenValueType.Infants, transferToken.Infants);
            _tokenValues.AddValue(TokenValueType.SupplierID, transferToken.SupplierID);

            char[] detailsBits = ConvertValuesToCharArray(_tokenValues.Values);

            string timeTokenString = new string(timeBits.Take(TokenLengths.Transfer).ToArray());
            string detailsTokenString = new string(detailsBits.Take(TokenLengths.Transfer).ToArray());

            string tokenString = timeTokenString + detailsTokenString.TrimEnd();

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

        public BookToken? DecodeBookToken(string tokenString)
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
            }
            catch (Exception ex)
            {
                token = null;
                _logger.LogError(ex, "BookTokenDecodeError");
            }

            return token;
        }

        public PropertyToken? DecodePropertyToken(string tokenString)
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
                    ISOCurrencyID = _tokenValues.GetValue(TokenValueType.CurrencyID)
                };

                int day = _tokenValues.GetValue(TokenValueType.Day);
                int month = _tokenValues.GetValue(TokenValueType.Month);
                int year = DateTime.Now.AddYears(_tokenValues.GetValue(TokenValueType.Year)).Year;

                if (day > 0 && month > 0)
                {
                    token.ArrivalDate = new DateTime(year, month, day);
                }
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
                _tokenValues.AddValue(TokenValueType.PropertyID);
                _tokenValues.AddValue(TokenValueType.MealBasisID);
                _tokenValues.AddValue(TokenValueType.Adults);
                _tokenValues.AddValue(TokenValueType.Children);
                _tokenValues.AddValue(TokenValueType.Infants);
                _tokenValues.AddValue(TokenValueType.PropertyRoomBookingID);

                GetTokenValues(tokenString.Substring(0, 8));

                token = new RoomToken()
                {
                    PropertyID = _tokenValues.GetValue(TokenValueType.PropertyID),
                    MealBasisID = _tokenValues.GetValue(TokenValueType.MealBasisID),
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

                _tokenValues.AddValue(TokenValueType.LocalCost2);
                _tokenValues.AddValue(TokenValueType.LocalCost1);

                GetTokenValues(new string(tokenString.Skip(14).ToArray()));
                token.LocalCost = GetLocalCostFromTokenValues();
            }
            catch (Exception ex)
            {
                token = null;
                _logger.LogError(ex, "RoomTokenDecodeError");
            }

            return token;
        }

        public TransferToken? DecodeTransferToken(string tokenString)
        {
            TransferToken? token = null;

            try
            {
                _tokenValues.Clear();
                _tokenValues.AddValue(TokenValueType.Year);
                _tokenValues.AddValue(TokenValueType.Month);
                _tokenValues.AddValue(TokenValueType.Day);
                _tokenValues.AddValue(TokenValueType.Hour1);
                _tokenValues.AddValue(TokenValueType.Minute1);
                _tokenValues.AddValue(TokenValueType.Duration);
                _tokenValues.AddValue(TokenValueType.OneWay);
                _tokenValues.AddValue(TokenValueType.Hour2);
                _tokenValues.AddValue(TokenValueType.Minute2);

                GetTokenValues(tokenString.Substring(0, 8));

                //TODO refactor this mess when token details settled
                token = new TransferToken()
                {
                    Duration = _tokenValues.GetValue(TokenValueType.Duration),
                    OneWay = _tokenValues.GetValue(TokenValueType.OneWay) == 1 ? true : false,
                };

                int day = _tokenValues.GetValue(TokenValueType.Day);
                int month = _tokenValues.GetValue(TokenValueType.Month);
                int year = DateTime.Now.AddYears(_tokenValues.GetValue(TokenValueType.Year)).Year;

                if (day > 0 && month > 0)
                {
                    token.DepartureDate = new DateTime(year, month, day);
                }

                int hour = _tokenValues.GetValue(TokenValueType.Hour1);
                int minute = _tokenValues.GetValue(TokenValueType.Minute1);

                token.DepartureTime = GetTimeFromTokenValues(hour, minute);

                if (!token.OneWay)
                {
                    hour = _tokenValues.GetValue(TokenValueType.Hour2);
                    minute = _tokenValues.GetValue(TokenValueType.Minute2);

                    token.ReturnTime = GetTimeFromTokenValues(hour, minute);
                }

                _tokenValues.Clear();
                _tokenValues.AddValue(TokenValueType.CurrencyID);
                _tokenValues.AddValue(TokenValueType.Adults);
                _tokenValues.AddValue(TokenValueType.Children);
                _tokenValues.AddValue(TokenValueType.Infants);
                _tokenValues.AddValue(TokenValueType.SupplierID);

                GetTokenValues(new string(tokenString.Substring(8, tokenString.Length - 8).ToArray()));

                token.ISOCurrencyID = _tokenValues.GetValue(TokenValueType.CurrencyID);
                token.Adults = _tokenValues.GetValue(TokenValueType.Adults);
                token.Children = _tokenValues.GetValue(TokenValueType.Children);
                token.Infants = _tokenValues.GetValue(TokenValueType.Infants);
                token.SupplierID = _tokenValues.GetValue(TokenValueType.SupplierID);
            }
            catch (Exception ex)
            {
                token = null;
                _logger.LogError(ex, "TransferTokenDecodeError");
            }

            return token;
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

        private void GenerateLocalCostValues(RoomToken roomToken)
        {
            _tokenValues.Clear();
            int[] localCosts = new int[2];
            var localCostSplit = SplitNumberToNDigitList((int)(roomToken.LocalCost * 100), 7);

            if (localCostSplit.Count < 2)
            {
                for (int i = 0; i < 2; ++i)
                {
                    if (i < 2 - localCostSplit.Count)
                    {
                        localCosts[i] = 0;
                    }
                    else
                    {
                        localCosts[i] = localCostSplit[i - 2 + localCostSplit.Count];
                    }
                }
            }
            else
            {
                localCosts = localCostSplit.ToArray();
            }

            for (int i = 2; i > 0; --i)
            {
                Enum.TryParse($"LocalCost{i}", out TokenValueType tokenValueName);
                _tokenValues.AddValue(tokenValueName, localCosts[i - 1]);
            }
        }

        private decimal GetLocalCostFromTokenValues()
        {
            var localCosts = new List<int>();

            for (int i = 0; i < 2; ++i)
            {
                Enum.TryParse($"LocalCost{i + 1}", out TokenValueType tokenValueName);

                int localCost = _tokenValues.GetValue(tokenValueName);
                localCosts.Add(localCost);
            }

            return GetNumFromList(localCosts, 7) / 100m;
        }

        /// <summary>
        /// Split the given integer into a list with elements of given digits
        /// </summary>
        /// <param name="number">A number which is going to be split</param>
        /// <param name="digit">A number of digits per element in the list</param>
        /// <returns>A list with number as an element of given length</returns>
        private List<int> SplitNumberToNDigitList(int number, int digit)
        {
            var list = new List<int>();
            var upperBound = Math.Pow(10, digit) - 1; // a max number which given n digit number can be i.e. if 2 digits then upperBound is 99
            var numOfDigits = (int)Math.Log10(number) + 1;

            if (number > upperBound)
            {
                var numberString = number.ToString();
                for (int i = 0; list.Count() < Math.Ceiling((decimal)numOfDigits / digit);)
                {
                    if (i == 0 && numOfDigits % digit != 0)
                    {
                        list.Add(int.Parse(numberString.Substring(i, 1)));
                        ++i;
                    }
                    else
                    {
                        list.Add(int.Parse(numberString.Substring(i, digit)));
                        i += digit;
                    }
                }
            }
            else
            {
                list.Add(number);
            }

            return list;
        }

        /// <summary>
        /// Get a combination of all numbers stored in the list as like string concatination 
        /// Pads a number with zeros if given number is less then specified num of digits
        /// </summary>
        /// <param name="list">A list storing numbers</param>
        /// <param name="digits">A number of digits of a number stored in each element</param>
        /// <returns>A number which is concatination of all numbers in a list</returns>
        private int GetNumFromList(List<int> list, int digits)
        {
            int numOfDecimals = (int)Math.Pow(10, digits);
            var number = 0;
            for (int i = 0; i < list.Count(); ++i)
            {
                number = number * numOfDecimals + list[i];
            }
            return number;
        }

        private (int, int) GetTimeParts(string time)
        {
            if (DateTime.TryParseExact(time, "t", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return (result.Hour, result.Minute);
            }

            return default;
        }

        private string GetTimeFromTokenValues(int hour, int minute)
        {
            return new DateTime(2022, 1, 1, hour, minute, 0).ToString("HH:mm").ToString();
        }

        public ExtraToken? DecodeExtraToken(string tokenString)
        {
            ExtraToken? token = null;

            try
            {
                _tokenValues.Clear();
                _tokenValues.AddValue(TokenValueType.Year);
                _tokenValues.AddValue(TokenValueType.Month);
                _tokenValues.AddValue(TokenValueType.Day);
                _tokenValues.AddValue(TokenValueType.Hour1);
                _tokenValues.AddValue(TokenValueType.Minute1);
                _tokenValues.AddValue(TokenValueType.Duration);
                _tokenValues.AddValue(TokenValueType.OneWay);
                _tokenValues.AddValue(TokenValueType.Hour2);
                _tokenValues.AddValue(TokenValueType.Minute2);

                GetTokenValues(tokenString.Substring(0, 8));

                //TODO refactor this mess when token details settled
                token = new ExtraToken()
                {
                    Duration = _tokenValues.GetValue(TokenValueType.Duration),
                    OneWay = _tokenValues.GetValue(TokenValueType.OneWay) == 1,
                };

                int day = _tokenValues.GetValue(TokenValueType.Day);
                int month = _tokenValues.GetValue(TokenValueType.Month);
                int year = DateTime.Now.AddYears(_tokenValues.GetValue(TokenValueType.Year)).Year;

                if (day > 0 && month > 0)
                {
                    token.DepartureDate = new DateTime(year, month, day);
                }

                int hour = _tokenValues.GetValue(TokenValueType.Hour1);
                int minute = _tokenValues.GetValue(TokenValueType.Minute1);

                token.DepartureTime = GetTimeFromTokenValues(hour, minute);

                if (!token.OneWay)
                {
                    hour = _tokenValues.GetValue(TokenValueType.Hour2);
                    minute = _tokenValues.GetValue(TokenValueType.Minute2);

                    token.ReturnTime = GetTimeFromTokenValues(hour, minute);
                }

                _tokenValues.Clear();
                _tokenValues.AddValue(TokenValueType.CurrencyID);
                _tokenValues.AddValue(TokenValueType.Adults);
                _tokenValues.AddValue(TokenValueType.Children);
                _tokenValues.AddValue(TokenValueType.Infants);
                _tokenValues.AddValue(TokenValueType.SupplierID);

                GetTokenValues(new string(tokenString.Substring(8, tokenString.Length - 8).ToArray()));

                token.ISOCurrencyID = _tokenValues.GetValue(TokenValueType.CurrencyID);
                token.Adults = _tokenValues.GetValue(TokenValueType.Adults);
                token.Children = _tokenValues.GetValue(TokenValueType.Children);
                token.Infants = _tokenValues.GetValue(TokenValueType.Infants);
                token.SupplierID = _tokenValues.GetValue(TokenValueType.SupplierID);
            }
            catch (Exception ex)
            {
                token = null;
                _logger.LogError(ex, "ExtraTokenDecodeError");
            }

            return token;
        }

        public string EncodeExtraToken(ExtraToken extraToken)
        {
            (int, int) departureTime = GetTimeParts(extraToken.DepartureTime);
            (int, int) returnTime = extraToken.OneWay ? (0, 0) : GetTimeParts(extraToken.ReturnTime);

            _tokenValues.Clear();
            _tokenValues.AddValue(TokenValueType.Year, extraToken.DepartureDate.Year - DateTime.Now.Year);
            _tokenValues.AddValue(TokenValueType.Month, extraToken.DepartureDate.Month);
            _tokenValues.AddValue(TokenValueType.Day, extraToken.DepartureDate.Day);
            _tokenValues.AddValue(TokenValueType.Hour1, departureTime.Item1);
            _tokenValues.AddValue(TokenValueType.Minute1, departureTime.Item2);
            _tokenValues.AddValue(TokenValueType.Duration, extraToken.Duration);
            _tokenValues.AddValue(TokenValueType.OneWay, extraToken.OneWay ? 1 : 0);
            _tokenValues.AddValue(TokenValueType.Hour2, returnTime.Item1);
            _tokenValues.AddValue(TokenValueType.Minute2, returnTime.Item2);

            char[] timeBits = ConvertValuesToCharArray(_tokenValues.Values);

            _tokenValues.Clear();
            _tokenValues.AddValue(TokenValueType.CurrencyID, extraToken.ISOCurrencyID);
            _tokenValues.AddValue(TokenValueType.Adults, extraToken.Adults);
            _tokenValues.AddValue(TokenValueType.Children, extraToken.Children);
            _tokenValues.AddValue(TokenValueType.Infants, extraToken.Infants);
            _tokenValues.AddValue(TokenValueType.SupplierID, extraToken.SupplierID);

            char[] detailsBits = ConvertValuesToCharArray(_tokenValues.Values);

            string timeTokenString = new string(timeBits.Take(TokenLengths.Transfer).ToArray());
            string detailsTokenString = new string(detailsBits.Take(TokenLengths.Transfer).ToArray());

            string tokenString = timeTokenString + detailsTokenString.TrimEnd();

            return tokenString;
        }
    }
}