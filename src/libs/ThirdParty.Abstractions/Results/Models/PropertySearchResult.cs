namespace ThirdParty.Results.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    /// <summary>
    /// a property search result
    /// </summary>
    public class PropertySearchResult
    {
        /// <summary>
        /// Gets or sets the property data.
        /// </summary>
        /// <value>
        /// The property data.
        /// </value>
        public PropertyData PropertyData { get; set; } = new PropertyData();

        /// <summary>
        /// Gets or sets the room results.
        /// </summary>
        /// <value>
        /// The room results.
        /// </value>
        public List<RoomSearchResult> RoomResults { get; set; } = new List<RoomSearchResult>();
    }
}
