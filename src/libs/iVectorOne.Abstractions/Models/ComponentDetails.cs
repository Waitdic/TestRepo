namespace iVectorOne.Models
{
    /// <summary>
    /// Component details
    /// </summary>
    public class ComponentDetails
    {
        /// <summary>
        /// Gets or sets the type of the component.
        /// </summary>
        public string ComponentType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        public string Reference { get; set; } = string.Empty;
    }
}