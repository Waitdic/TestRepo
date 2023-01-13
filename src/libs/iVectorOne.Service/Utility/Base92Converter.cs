namespace iVectorOne.Utility
{
    using System;

    public class Base92Converter : IBaseConverter
    {
        private const int CharStartIndex = 32;
        private const int NinetyTwo = 92;
        private const int NinetyTwoSquared = 8464;
        private const int NinetyTwoCubed = 778688;
        private const long NinetyTwoToFourthPower = 71639296;
        private const long NinetyTwoToFifthPower = 6590815232;
        private const long NinetyTwoToSixthPower = 606355001344;
        private const long NinetyTwoToSeventhPower = 55784660123648;
        private const long NinetyTwoToEighthPower = 5132188731375616;

        /// <summary>
        /// This is 92^8-1
        /// </summary>
        private const long MaxValue = 5132188731375615;

        public char[] Encode(long totalDecimal)
        {
            if (totalDecimal < 0 || totalDecimal > MaxValue)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(totalDecimal),
                    totalDecimal,
                    "The value to encode is outside of the range of allowed values.");
            }

            long remainder = 0;

            long bit8 = totalDecimal / NinetyTwoToEighthPower;
            remainder += bit8 * NinetyTwoToEighthPower;
            long bit7 = (totalDecimal - remainder) / NinetyTwoToSeventhPower;
            remainder += bit7 * NinetyTwoToSeventhPower;
            long bit6 = (totalDecimal - remainder) / NinetyTwoToSixthPower;
            remainder += bit6 * NinetyTwoToSixthPower;
            long bit5 = (totalDecimal - remainder) / NinetyTwoToFifthPower;
            remainder += bit5 * NinetyTwoToFifthPower;
            long bit4 = (totalDecimal - remainder) / NinetyTwoToFourthPower;
            remainder += bit4 * NinetyTwoToFourthPower;
            long bit3 = (totalDecimal - remainder) / NinetyTwoCubed;
            remainder += bit3 * NinetyTwoCubed;
            long bit2 = (totalDecimal - remainder) / NinetyTwoSquared;
            remainder += bit2 * NinetyTwoSquared;
            long bit1 = (totalDecimal - remainder) / NinetyTwo;
            remainder += bit1 * NinetyTwo;
            long bit0 = totalDecimal - remainder;

            char[] bits =
            {
                (char) (bit0 + CharStartIndex),
                (char) (bit1 + CharStartIndex),
                (char) (bit2 + CharStartIndex),
                (char) (bit3 + CharStartIndex),
                (char) (bit4 + CharStartIndex),
                (char) (bit5 + CharStartIndex),
                (char) (bit6 + CharStartIndex),
                (char) (bit7 + CharStartIndex),
                (char) (bit8 + CharStartIndex)
            };
            return bits;
        }

        public long Decode(string tokenString)
        {
            var chars = tokenString.ToCharArray();

            long decimalValue = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                decimalValue += (chars[i] - CharStartIndex) * (long)System.Numerics.BigInteger.Pow(NinetyTwo, i);
            }

            return decimalValue;
        }
    }
}