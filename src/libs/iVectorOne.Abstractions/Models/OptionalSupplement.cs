namespace ThirdParty.Models
{
    /// <summary>
    /// An Optional Supplement
    /// </summary>
    public class OptionalSupplement
    {
        /// <summary>
        /// Gets or sets the contract supplement identifier.
        /// </summary>
        /// <value>
        /// The contract supplement identifier.
        /// </value>
        public int ContractSupplementID { get; set; }

        /// <summary>
        /// Gets or sets the supplement.
        /// </summary>
        /// <value>
        /// The supplement.
        /// </value>
        public string Supplement { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of the passenger.
        /// </summary>
        /// <value>
        /// The type of the passenger.
        /// </value>
        public string PaxType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether [payable local].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [payable local]; otherwise, <c>false</c>.
        /// </value>
        public bool PayableLocal { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OptionalSupplement"/> is mandatory.
        /// </summary>
        /// <value>
        ///   <c>true</c> if mandatory; otherwise, <c>false</c>.
        /// </value>
        public bool Mandatory { get; set; }

        /// <summary>
        /// Gets or sets the rate calculation.
        /// </summary>
        /// <value>
        /// The rate calculation.
        /// </value>
        public string RateCalculation { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public decimal Value { get; set; }

        /// <summary>
        /// Gets or sets the adult.
        /// </summary>
        /// <value>
        /// The adult.
        /// </value>
        public decimal Adult { get; set; }

        /// <summary>
        /// Gets or sets the child.
        /// </summary>
        /// <value>
        /// The child.
        /// </value>
        public decimal Child { get; set; }

        /// <summary>
        /// Gets or sets the infant.
        /// </summary>
        /// <value>
        /// The infant.
        /// </value>
        public decimal Infant { get; set; }
    }
}
