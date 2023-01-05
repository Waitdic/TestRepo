namespace iVectorOne.SDK.V2.TransferBook
{
    /// <summary>
    /// a class representing journey details as part of a transfer booking
    /// </summary>
    public class JourneyDetails
    {
        /// <summary>
        /// Gets or sets the flight code.
        /// </summary>
        public string FlightCode { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the accommodation name.
        /// </summary>
        public string AccommodationName { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the train details.
        /// </summary>
        public string TrainDetails { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the vessel name.
        /// </summary>
        public string VesselName { get; set; } = string.Empty;
    }
}