namespace iVectorOne.Search.Results.Models
{
    using System.Xml.Serialization;
    using iVectorOne.SDK.V2.PropertySearch;

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

        protected TransformedResultAdjustment()
        {
        }

        /// <summary>
        /// Gets or sets the type of the adjustment.
        /// </summary>
        [XmlAttribute("AT")]
        public AdjustmentType AdjustmentType { get; set; } 

        /// <summary>
        /// Gets or sets the name of the adjustment.
        /// </summary>
        [XmlAttribute("AN")]
        public string AdjustmentName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the adjustment amount.
        /// </summary>
        [XmlAttribute("AA")]
        public decimal AdjustmentAmount { get; set; }

        /// <summary>
        /// Gets or sets the adjustment description.
        /// </summary>
        [XmlAttribute("DSC")]
        public string AdjustmentDescription { get; set; } = string.Empty;
    }
}