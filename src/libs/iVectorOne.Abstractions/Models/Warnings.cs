namespace iVectorOne.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// A list of warnings
    /// </summary>
    /// <seealso cref="List{Warning}" />
    public class Warnings : List<Warning>
    {
        /// <summary>
        /// Adds the new.
        /// </summary>
        /// <param name="title">The s title.</param>
        /// <param name="text">The s text.</param>
        public void AddNew(string title, string text)
        {
            this.Add(new Warning(title, text));
        }

        /// <summary>
        /// Adds the new.
        /// </summary>
        /// <param name="title">The s title.</param>
        /// <param name="text">The s text.</param>
        /// <param name="type">Type of the s.</param>
        public void AddNew(string title, string text, WarningType type)
        {
            this.Add(new Warning(title, text, type));
        }
    }
}
