using System.Collections.Generic;

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
        /// <summary>
        /// Gets or sets additional departure data.
        /// </summary>
        public List<string> AdditionalDepartureData { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets additional arrival data.
        /// </summary>
        public List<string> AdditionalArrivalData { get; set; } = new List<string>();
    }
}