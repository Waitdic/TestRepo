namespace ThirdParty.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using iVector.Search.Property;

    /// <summary>A collection of resort splits for each supplier</summary>
    public class SupplierResortSplit
    {
        /// <summary>Gets or sets the supplier associated with this group of resort splits</summary>
        /// <value>The supplier.</value>
        public string Supplier { get; set; } = string.Empty;

        /// <summary>Gets or sets the resort splits to be searched</summary>
        /// <value>The resort splits.</value>
        public List<ResortSplit> ResortSplits { get; set; } = new List<ResortSplit>();

        /// <summary>Gets or sets a value indicating whether [use interface].</summary>
        /// <value>
        /// <c>true</c> if [use interface]; otherwise, <c>false</c>.</value>
        public bool UseInterface { get; set; }

        /// <summary>Returns all hotels for this supplier contained within its resort splits.</summary>
        /// <value>All hotels for this supplier contained within its resort splits.</value>
        public List<Hotel> AllHotels { 
            get {
                return this.ResortSplits.SelectMany(rs => rs.Hotels).ToList();    
            } 
        }
    }
}
