using iVectorOne.SDK.V2.LocationContent;
using System.Collections.Generic;
using System.Linq;

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

        public bool IsValid()
        {
            return DepartureData != null &&
                ArrivalData != null &&
                DepartureData.Length > 0 &&
                ArrivalData.Length > 0 &&
                AdditionalArrivalData.All(x => x.Length > 0) &&
                AdditionalDepartureData.All(x => x.Length > 0);
        }
    }
}