namespace iVectorOne.Models.Tokens.Constants
{
    /// <summary>Class containing constants for how many bits are required to store certain Token values.</summary>
    public class TokenValueBits
    {
        /// <summary>PropertyIDS stored in 24 bits, allows 16,777,511 values to be stored. Currently there are
        /// 5.3 million properties in the system so plenty of room to grow</summary>
        public const int PropertyIDBits = 24;

        /// <summary>
        ///   <para>Years are stored in 2 bits, and we store how many years ahead of us the year is, so we support 0-3 years ahead</para>
        /// </summary>
        public const int YearBits = 2;

        /// <summary>Months are stored in 4 bits, which allows 0-15 to be stored (obviously we will only use 1-12)</summary>
        public const int MonthBits = 4;

        /// <summary>Days are stored in 5 bits, which allows 0-31 (perfect) to be stored.</summary>
        public const int DayBits = 5;

        /// <summary>
        ///   <para>Durations are stored in 6 bits, which allows 0-63 to be stored, if we need to support larger searches than this, we will need to increase it.</para>
        /// </summary>
        public const int DurationBits = 6;

        /// <summary>
        ///   <para>Adults are stored in 4 bits, which allows values 0-15 to be stored.</para>
        /// </summary>
        public const int AdultsBits = 4;

        /// <summary>Children are stored in 4 bits, which allows values 0-15 to be stored. Thouh we only actually support 8 and validate against more.</summary>
        public const int ChildrenBits = 4;

        /// <summary>Infants are stored in 3 bits, which allows values 0-7 to be stored.</summary>
        public const int InfantsBits = 3;

        /// <summary>Rooms are stored in 3 bits, which allows values 0-7 to be stored.</summary>
        public const int RoomBits = 3;

        /// <summary>The currency ids are stored in 9, which allows 0-511 to be stored(But not going to all used.There are 300 ISO currency codes )</summary>
        public const int CurrencyBits = 9;

        /// <summary>Child ages are stored in 5 bits which allow 0-31 to be stored (though we validate these to 1-17)</summary>
        public const int ChildAgeBits = 5;

        /// <summary>LocalCost will be stored as a split of 2 parts of 7 digits, each part is stored in 24 bits which allow 
        /// 0-16,777,216 to be stored (only 0-9,999,999 will be used)</summary>
        public const int LocalCostBits = 24;

        /// <summary>Property room booking ids are stored in 4 bits, which allows 0-15 to be stored</summary>
        public const int PropertyRoomBookingID = 4;

        /// <summary>MealBasis ids are stored in 12 bits which allow 0-4095 to be stored</summary>
        public const int MealBasis = 12;
    }
}