namespace iVectorOne.Models.Property
{
    /// <summary>
    /// an option
    /// </summary>
    public class Option
    {
        /// <summary>
        /// Gets or sets the option identifier.
        /// </summary>
        public int OptionID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        public int SortOrder { get; set; }
    }
}