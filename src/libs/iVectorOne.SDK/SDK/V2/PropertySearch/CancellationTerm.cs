namespace iVectorOne.SDK.V2.PropertySearch
{
    using System;

    /// <summary>
    /// A class representing the cancellation terms for a date band for a room type
    /// </summary>
    public class CancellationTerm
    {
        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public decimal Amount { get; set; }
    }
}