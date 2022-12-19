namespace iVectorOne.SDK.V2.PropertyPrebook
{
    /// <summary>
    /// a class representing a single room booking made as part of a property booking
    /// </summary>
    public class RoomBooking
    {
        /// <summary>
        /// Gets or sets the supplier.
        /// </summary>
        public string Supplier { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier reference.</summary>
        public string SupplierReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total cost.
        /// </summary>
        public decimal TotalCost { get; set; }

        /// <summary>
        /// Gets or sets the minimum selling price.
        /// </summary>
        public decimal MinimumSellingPrice { get; set; }

        /// <summary>
        /// Gets or sets the commission percentage.
        /// </summary>
        public decimal CommissionPercentage { get; set; }

        /// <summary>
        /// Gets or sets the room booking token.
        /// </summary>
        public string RoomBookingToken { get; set; } = string.Empty;
    }
}