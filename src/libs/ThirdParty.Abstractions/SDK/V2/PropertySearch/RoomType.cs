namespace ThirdParty.SDK.V2.PropertySearch
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    /// A class representing a room on a property result
    /// </summary>
    public class RoomType
    {
        /// <summary>
        /// Gets or sets the room identifier.
        /// </summary>
        /// <value>
        /// The room identifier.
        /// </value>
        public int RoomID { get; set; }

        /// <summary>
        /// Gets or sets the room booking token.
        /// </summary>
        /// <value>
        /// The room booking token.
        /// </value>
        public string RoomBookingToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the supplier.
        /// </summary>
        /// <value>
        /// The supplier.
        /// </value>
        public string Supplier { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the meal basis code.
        /// </summary>
        /// <value>
        /// The meal basis code.
        /// </value>
        public string MealBasisCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonPropertyName("RoomType")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [JsonPropertyName("RoomTypeCode")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of the supplier room.
        /// </summary>
        /// <value>
        /// The type of the supplier room.
        /// </value>
        public string SupplierRoomType { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier reference 1.</summary>
        /// <value>The supplier reference.</value>
        public string SupplierReference1 { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier reference 2.</summary>
        /// <value>The supplier reference.</value>
        public string SupplierReference2 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        /// <value>
        /// The currency code.
        /// </value>
        public string CurrencyCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total cost.
        /// </summary>
        /// <value>
        /// The total cost.
        /// </value>
        public decimal TotalCost { get; set; }

        /// <summary>
        /// Gets or sets the discount.
        /// </summary>
        /// <value>
        /// The discount.
        /// </value>
        public decimal Discount { get; set; }

        /// <summary>
        /// Gets or sets the third party rate code.
        /// </summary>
        /// <value>
        /// The third party rate code.
        /// </value>
        public string TPRateCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether [non refundable].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [non refundable]; otherwise, <c>false</c>.
        /// </value>
        public bool NonRefundable { get; set; }

        /// <summary>
        /// Gets or sets the cancellation terms.
        /// </summary>
        /// <value>
        /// The cancellation terms.
        /// </value>
        public List<CancellationTerm> CancellationTerms { get; set; } = new List<CancellationTerm>();

        /// <summary>
        /// Gets or sets the adjustments.
        /// </summary>
        /// <value>
        /// The adjustments.
        /// </value>
        public List<Adjustment> Adjustments { get; set; } = new List<Adjustment>();


        /// <summary>
        /// Gets or sets the commission percentage.
        /// </summary>
        /// <value>
        /// The commission percentage.
        /// </value>
        public decimal CommissionPercentage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [on request].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [on request]; otherwise, <c>false</c>.
        /// </value>
        public bool OnRequest { get; set; }

    }
}