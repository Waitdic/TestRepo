namespace iVectorOne.Models
{
    /// <summary>
    /// A location mapping
    /// </summary>
    public class LocationMapping
    {
        /// <summary>
        /// Gets or sets the departure data.
        /// </summary>
        public string DepartureData { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the arrival data.
        /// </summary>
        public string ArrivalData { get; set; } = string.Empty;
    }
}