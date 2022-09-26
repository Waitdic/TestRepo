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

        public Adjustment() { }

        /// <summary>
        /// Gets or sets the adjustment type.
        /// </summary>
        public AdjustmentType AdjustmentType { get; set; }

        /// <summary>
        /// Gets or sets the adjustment name.
        /// </summary>
        public string AdjustmentName { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public decimal AdjustmentAmount { get; set; }

        /// <summary>
        /// Gets or sets the adjustment description.
        /// </summary>
        public string Description { get; set; }
    }
}