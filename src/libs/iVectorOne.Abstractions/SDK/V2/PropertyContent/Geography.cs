namespace iVectorOne.SDK.V2.PropertyContent
{
    /// <summary>Class containing the information about the geography of a property</summary>
    public class Geography
    {
        /// <summary>Gets or sets the country.</summary>
        /// <value>The country.</value>
        public string Country { get; set; } = string.Empty;

        /// <summary>Gets or sets the region.</summary>
        /// <value>The region.</value>
        public string Region { get; set; } = string.Empty;

        /// <summary>Gets or sets the resort.</summary>
        /// <value>The resort.</value>
        public string Resort { get; set; } = string.Empty;

        /// <summary>Gets or sets the latitude.</summary>
        /// <value>The latitude.</value>
        public decimal Latitude { get; set; }

        /// <summary>Gets or sets the longitude.</summary>
        /// <value>The longitude.</value>
        public decimal Longitude { get; set; }

        /// <summary>Gets or sets the code.</summary>
        /// <value>The longitude.</value>
        public string Code { get; set; }
    }
}