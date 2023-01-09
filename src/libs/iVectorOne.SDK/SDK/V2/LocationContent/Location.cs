namespace iVectorOne.SDK.V2.LocationContent
{
    /// <summary>
    /// A class representing a single property result 
    /// </summary>
    public class Location
    {
        /// <summary> Gets or sets the location id. </summary>
        public int LocationID { get; set; }

        /// <summary> Gets or sets the description. </summary>
        public string Description { get; set; } = string.Empty;
    }
}
