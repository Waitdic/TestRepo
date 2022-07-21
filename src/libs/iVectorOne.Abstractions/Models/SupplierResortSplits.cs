namespace ThirdParty.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using iVector.Search.Property;

    /// <summary>A collection of resort splits for each supplier</summary>
    public class SupplierResortSplit
    {
        /// <summary>Gets or sets the supplier associated with this group of resort splits</summary>
        /// <value>The supplier.</value>
        public string Supplier { get; set; } = string.Empty;

        /// <summary>Gets or sets the resort splits to be searched</summary>
        /// <value>The resort splits.</value>
        public List<ResortSplit> ResortSplits { get; set; } = new();

        /// <summary>Returns all hotels for this supplier contained within its resort splits.</summary>
        /// <value>All hotels for this supplier contained within its resort splits.</value>
        public List<Hotel> AllHotels => this.ResortSplits.SelectMany(rs => rs.Hotels).ToList();
    }
}
