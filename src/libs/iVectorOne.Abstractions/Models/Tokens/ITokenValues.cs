namespace iVectorOne.Models.Tokens
{
    using System.Collections.Generic;

    /// <summary>A collection of token values, made as a class rather than just a list to allow utiltiy functions for adding new values</summary>
    public interface ITokenValues
    {
        /// <summary>Gets the values.</summary>
        List<TokenValue> Values { get; }

        /// <summary>Add value method for when you don't know the value yet</summary>
        /// <param name="name">The name of the value</param>
        void AddValue(TokenValueType name);

        /// <summary>Sets up and adds a new value.</summary>
        /// <param name="name">The name of the value.</param>
        /// <param name="value">The value being stored.</param>
        void AddValue(TokenValueType name, int value);

        /// <summary>Returns a value that matches the passed in name if one exists</summary>
        /// <param name="name">The name of the value you want to retreive</param>
        /// <returns>The integer Value stored in the collection associated with the name.</returns>
        int GetValue(TokenValueType name);

        /// <summary>Empties the collection.</summary>
        void Clear();
    }
}