namespace iVectorOne.SDK.V2.ExtraPrebook
{
    using System.Collections.Generic;

    public record Response : ResponseBase
    {
        /// <summary>
        /// Gets or sets the booking token.
        /// </summary>
        public string BookingToken { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier reference.</summary>
        public string SupplierReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total cost.
        /// </summary>
        public decimal TotalCost { get; set; }

        /// <summary>
        /// Gets or sets the cancellation terms.
        /// </summary>
        public List<CancellationTerm> CancellationTerms { get; set; } = new();

        /// <summary>
        /// Gets or sets the errata.
        /// </summary>
        public List<string> Errata { get; set; } = new List<string>();
    }
}