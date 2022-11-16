namespace iVectorOne.SDK.V2.TransferPrebook
{
    using System.Collections.Generic;
    //using iVectorOne.SDK.V2.PropertySearch;

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
        /// Gets or sets the departure notes.
        /// </summary>
        public string DepartureNotes { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the return notes.
        /// </summary>
        public string ReturnNotes { get; set; } = string.Empty;

    }
}