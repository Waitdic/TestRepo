namespace iVectorOne.Models.Transfer
{
    using System;
    using System.Collections.Generic;
    using Intuitive.Helpers.Net;
    using iVectorOne.Models.SupplierLog;

    public class JourneyDetails
    {
        /// <summary>
        /// Gets or sets The flight code
        /// </summary>
        public string FlightCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the AccommodationName.
        /// </summary>
        public string AccommodationName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the TrainDetails.
        /// </summary>
        public string TrainDetails { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the VesselName.
        /// </summary>
        public string VesselName { get; set; } = string.Empty;
    }
}