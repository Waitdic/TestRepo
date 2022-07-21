namespace iVectorOne.Models.Tokens
{
    /// <summary>Represents a value in a base 92 token</summary>
    public class TokenValue
    {
        /// <summary>Gets or sets the type.</summary>
        /// <value>The name identifies the data being stored in this partof the token e.g. a duration</value>
        public TokenValueType Type { get; set; }

        /// <summary>Gets or sets the bits.</summary>
        /// <value>
        /// The bits required to store this number as binary. To calculate find the range, and then
        /// its LOGe(range) / LOGe(2), remember its zero indexed, so for something that can be 0 - 7 you
        /// calculate it as LOGe(8) / LOGe(2) = 3 bits.
        /// </value>
        public int Bits { get; set; }

        /// <summary>Gets or sets the start position.</summary>
        /// <value>
        /// The start position is the position in the bit string that this value will start at (this is usually automatically calculated), adding a new item, you want it to be the previous items start Position + its btis
        /// </value>
        public int StartPosition { get; set; }

        /// <summary>Gets or sets the value.</summary>
        /// <value>The value being stored</value>
        public int Value { get; set; }
    }
}