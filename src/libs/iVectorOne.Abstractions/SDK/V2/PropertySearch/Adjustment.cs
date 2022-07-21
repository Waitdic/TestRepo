namespace iVectorOne.SDK.V2.PropertySearch
{
    /// <summary>
    /// A class representing the adjustment for a room type
    /// </summary>
    public class Adjustment
    {
        public Adjustment(AdjustmentType type, string name, string description, decimal amount = 0)
        {
            AdjustmentType = type;
            AdjustmentName = name;
            AdjustmentAmount = amount;
            Description = description;
        }

        /// <summary>
        /// Gets or sets the adjustment type.
        /// </summary>
        /// <value>
        /// The adjustment type.
        /// </value>
        public AdjustmentType AdjustmentType { get; set; }

        /// <summary>
        /// Gets or sets the adjustment name.
        /// </summary>
        /// <value>
        /// The adjustment name.
        /// </value>
        public string AdjustmentName { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        public decimal AdjustmentAmount { get; set; }

        /// <summary>
        /// Gets or sets the adjustment description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }
    }
}