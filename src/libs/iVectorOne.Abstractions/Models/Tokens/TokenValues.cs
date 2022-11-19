namespace iVectorOne.Models.Tokens
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using iVectorOne.Models.Tokens.Constants;

    /// <summary>A collection of token values, made as a class rather than just a list to allow utiltiy functions for adding new values</summary>
    public class TokenValues : ITokenValues
    {
        /// <summary>The values</summary>
        private List<TokenValue> _values = new();

        /// <summary>Gets the stored token values.</summary>
        public List<TokenValue> Values
        {
            get
            {
                return _values;
            }
        }

        /// <summary>Sets up and adds a new value.</summary>
        /// <param name="type">The name of the value.</param>
        /// <param name="value">The value being stored.</param>
        public void AddValue(TokenValueType type, int value)
        {
            var startPosition = 0;

            if(this._values.Any(v => v.Type == type))
            {
                throw new ArgumentException("A value of that type is already in the collection", nameof(type));
            } 
            else if (this._values.Any())
            {
                var latest = this._values.Last();
                startPosition = latest.StartPosition + latest.Bits;
            }

            var bits = GetBits(type);

            if (Math.Log(value,2) >= bits)
            {
                throw new ArgumentOutOfRangeException("Value");
            }

            var newValue = new TokenValue()
            {
                Type = type,
                Bits = bits,
                StartPosition = startPosition,
                Value = value
            };

            this._values.Add(newValue);
        }

        /// <summary>Add value method for when you don't know the value yet</summary>
        /// <param name="name">The name of the value</param>
        public void AddValue(TokenValueType name)
        {
            this.AddValue(name, 0);
        }

        /// <summary>Empties the collection.</summary>
        public void Clear()
        {
            this._values = new List<TokenValue>();
        }

        /// <summary>Returns a value that matches the passed in name if one exists</summary>
        /// <param name="name">The name of the value you want to retreive</param>
        /// <returns>The integer Value stored in the collection associated with the name.</returns>
        public int GetValue(TokenValueType name)
        {
            var value = 0;

            if (this.Values.Any(v => v.Type == name))
            {
                value = this.Values.FirstOrDefault(v => v.Type == name).Value;
            }

            return value;
        }


        /// <summary>Returns the number of bits required to store a token value</summary>
        /// <param name="type">The type of value to be stored</param>
        /// <returns>The number of bits required to store the type</returns>
        private int GetBits(TokenValueType type)
        {
            var bits = 0;

            switch (type)
            {
                case TokenValueType.PropertyID:
                    bits = TokenValueBits.PropertyIDBits;
                    break;
                case TokenValueType.Year:
                    bits = TokenValueBits.YearBits;
                    break;
                case TokenValueType.Month:
                    bits = TokenValueBits.MonthBits;
                    break;
                case TokenValueType.Day:
                    bits = TokenValueBits.DayBits;
                    break;
                case TokenValueType.Hour1:
                case TokenValueType.Hour2:
                    bits = TokenValueBits.HourBits;
                    break;
                case TokenValueType.Minute1:
                case TokenValueType.Minute2:
                    bits = TokenValueBits.MinuteBits;
                    break;
                case TokenValueType.Duration:
                    bits = TokenValueBits.DurationBits;
                    break;
                case TokenValueType.Adults:
                    bits = TokenValueBits.AdultsBits;
                    break;
                case TokenValueType.Children:
                    bits = TokenValueBits.ChildrenBits;
                    break;
                case TokenValueType.Rooms:
                    bits = TokenValueBits.RoomBits;
                    break;
                case TokenValueType.Infants:
                    bits = TokenValueBits.InfantsBits;
                    break;
                case TokenValueType.LocalCost1:
                case TokenValueType.LocalCost2:
                    bits = TokenValueBits.LocalCostBits;
                    break;
                case TokenValueType.PropertyRoomBookingID:
                    bits = TokenValueBits.PropertyRoomBookingID;
                    break;
                case TokenValueType.CurrencyID:
                    bits = TokenValueBits.CurrencyBits;
                    break;
                case TokenValueType.MealBasisID1:
                case TokenValueType.MealBasisID2:
                case TokenValueType.MealBasisID3:
                    bits = TokenValueBits.MealBasis;
                    break;
                case TokenValueType.SupplierID:
                    bits = TokenValueBits.SupplierBits;
                    break;
                case TokenValueType.ChildAge1:
                case TokenValueType.ChildAge2:
                case TokenValueType.ChildAge3:
                case TokenValueType.ChildAge4:
                case TokenValueType.ChildAge5:
                case TokenValueType.ChildAge6:
                case TokenValueType.ChildAge7:
                case TokenValueType.ChildAge8:
                    bits = TokenValueBits.ChildAgeBits;
                    break;
                case TokenValueType.OneWay:
                    bits = TokenValueBits.OneWayBits;
                    break;
                default:
                    break;
            }

            return bits;
        }
    }
}
