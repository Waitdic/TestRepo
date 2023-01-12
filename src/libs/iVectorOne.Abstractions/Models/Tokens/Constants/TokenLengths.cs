namespace iVectorOne.Models.Tokens.Constants
{
    /// <summary>Class containing the expected length of the tokens, used to trim off the whitespace</summary>
    public class TokenLengths
    {
        /// <summary>Room tokens are 8 characters</summary>
        public const int Room = 8;

        /// <summary>ChildAges tokens are 6 characters</summary>
        public const int ChildAges = 6;

        /// <summary>Property tokens are 8 characters</summary>
        public const int Property = 8;

        /// <summary>Book tokens are 4 characters</summary>
        public const int Book = 5;

        /// <summary>MealBasis tokens are 4 characters</summary>
        public const int MealBasis = 4;

        /// <summary>Local cost tokens are 8 characters (supports local costs up to 14 digits with 2 decimal places)</summary>
        public const int LocalCost = 8;

        /// <summary>Transfer tokens are 8 characters</summary>
        public const int Transfer = 8;
    }
}
