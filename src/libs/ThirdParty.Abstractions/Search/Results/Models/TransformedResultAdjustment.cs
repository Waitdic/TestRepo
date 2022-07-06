namespace ThirdParty.Search.Results.Models
{
    using System.Xml.Serialization;
    using ThirdParty.SDK.V2.PropertySearch;

    /// <summary>
    /// The transformed result adjustment
    /// </summary>
    public class TransformedResultAdjustment
    {
        public TransformedResultAdjustment(AdjustmentType type, string name, string description, decimal amount = 0)
        {
            AdjustmentType = type;
            AdjustmentName = name;
            AdjustmentDescription = description;
            AdjustmentAmount = amount;
        }
        /// <summary>
        /// Gets or sets the adjustment identifier.
        /// </summary>
        /// <value>
        /// The adjustment identifier.
        /// </value>
        [XmlAttribute("AID")]
        public int AdjustmentID { get; set; }

        /// <summary>
        /// Gets or sets the type of the adjustment.
        /// </summary>
        /// <value>
        /// The type of the adjustment.
        /// </value>
        [XmlAttribute("AT")]
        public AdjustmentType AdjustmentType { get; set; } 

        /// <summary>
        /// Gets or sets the name of the adjustment.
        /// </summary>
        /// <value>
        /// The name of the adjustment.
        /// </value>
        [XmlAttribute("AN")]
        public string AdjustmentName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the adjustment amount.
        /// </summary>
        /// <value>
        /// The adjustment amount.
        /// </value>
        [XmlAttribute("AA")]
        public decimal AdjustmentAmount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [pay local].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [pay local]; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute("PL")]
        public bool PayLocal { get; set; }

        /// <summary>
        /// Gets or sets the adjustment description.
        /// </summary>
        /// <value>
        /// The adjustment description.
        /// </value>
        [XmlAttribute("DSC")]
        public string AdjustmentDescription { get; set; } = string.Empty;
    }
}
