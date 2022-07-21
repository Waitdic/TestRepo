namespace ThirdParty.Models
{
    /// <summary>
    /// an option
    /// </summary>
    public class Option
    {
        /// <summary>
        /// Gets or sets the option identifier.
        /// </summary>
        /// <value>
        /// The option identifier.
        /// </value>
        public int OptionID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>
        /// The sort order.
        /// </value>
        public int SortOrder { get; set; }
    }
}