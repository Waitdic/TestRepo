namespace iVectorOne.Models
{
    /// <summary>
    /// An Optional Supplement
    /// </summary>
    public class OptionalSupplement
    {
        /// <summary>
        /// Gets or sets the contract supplement identifier.
        /// </summary>
        public int ContractSupplementID { get; set; }

        /// <summary>
        /// Gets or sets the supplement.
        /// </summary>
        public string Supplement { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of the passenger.
        /// </summary>
        public string PaxType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether [payable local].
        /// </summary>
        public bool PayableLocal { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OptionalSupplement"/> is mandatory.
        /// </summary>
        public bool Mandatory { get; set; }

        /// <summary>
        /// Gets or sets the rate calculation.
        /// </summary>
        public string RateCalculation { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// Gets or sets the adult.
        /// </summary>
        public decimal Adult { get; set; }

        /// <summary>
        /// Gets or sets the child.
        /// </summary>
        public decimal Child { get; set; }

        /// <summary>
        /// Gets or sets the infant.
        /// </summary>
        public decimal Infant { get; set; }
    }
}