namespace iVectorOne.Utility
{
    /// <summary>Interface for a class that encodes and decodes to a difference base</summary>
    public interface IBaseConverter
    {
        /// <summary>Takes in a long, and converts it into a char string in the new base</summary>
        /// <param name="total">The total decimal.</param>
        /// <returns>A char string encoding the input in the new base.</returns>
        char[] Encode(long total);

        /// <summary>Decodes the specified token string.</summary>
        /// <param name="tokenString">The token string.</param>
        /// <returns>Decodes the token string into the integer value being stored.</returns>
        long Decode(string tokenString);
    }
}