namespace iVectorOne.Models.Tokens
{
    /// <summary>Represents a value in a base 92 token</summary>
    public class TokenValue
    {
        /// <summary>Gets or sets the identifier of the data being stored in this partof the token e.g. a duration.</summary>
        public TokenValueType Type { get; set; }

        /// <summary>
        /// Gets or sets the bits required to store this number as binary. To calculate find the range, and then
        /// its LOGe(range) / LOGe(2), remember its zero indexed, so for something that can be 0 - 7 you
        /// calculate it as LOGe(8) / LOGe(2) = 3 bits.
        /// </summary>
        public int Bits { get; set; }

        /// <summary>
        /// Gets or sets the the position in the bit string that this value will start at (this is usually automatically calculated), adding a new item, you want it to be the previous items start Position + its btis
        /// </summary>
        public int StartPosition { get; set; }

        /// <summary>Gets or sets the value being stored.</summary>
        public int Value { get; set; }
    }
}