namespace iVectorOne.Models.Property
{
    using System.Collections.Generic;
    using System.Linq;
    using iVector.Search.Property;

    /// <summary>A collection of resort splits for each supplier</summary>
    public class SupplierResortSplit
    {
        /// <summary>Gets or sets the supplier associated with this group of resort splits</summary>
        public string Supplier { get; set; } = string.Empty;

        /// <summary>Gets or sets the resort splits to be searched</summary>
        public List<ResortSplit> ResortSplits { get; set; } = new();

        /// <summary>Returns all hotels for this supplier contained within its resort splits.</summary>
        public List<Hotel> AllHotels => this.ResortSplits.SelectMany(rs => rs.Hotels).ToList();
    }
}