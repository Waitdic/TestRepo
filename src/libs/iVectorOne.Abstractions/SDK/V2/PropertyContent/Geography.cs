namespace iVectorOne.SDK.V2.PropertyContent
{
    /// <summary>Class containing the information about the geography of a property</summary>
    public class Geography
    {
        /// <summary>Gets or sets the country.</summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>Gets or sets the region.</summary>
        public string Region { get; set; } = string.Empty;

        /// <summary>Gets or sets the resort.</summary>
        public string Resort { get; set; } = string.Empty;

        /// <summary>Gets or sets the latitude.</summary>
        public decimal Latitude { get; set; }

        /// <summary>Gets or sets the longitude.</summary>
        public decimal Longitude { get; set; }

        /// <summary>Gets or sets the code.</summary>
        public string Code { get; set; } = string.Empty;
    }
}