namespace iVectorOne.SDK.V2.PropertySearch
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
        public int RoomID { get; set; }

        /// <summary>
        /// Gets or sets the room booking token.
        /// </summary>
        public string RoomBookingToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the supplier.
        /// </summary>
        public string Supplier { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the meal basis code.
        /// </summary>
        public string MealBasisCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [JsonPropertyName("RoomType")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        [JsonPropertyName("RoomTypeCode")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of the supplier room.
        /// </summary>
        public string SupplierRoomType { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier reference 1.</summary>
        public string SupplierReference1 { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier reference 2.</summary>
        public string SupplierReference2 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        public string CurrencyCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total cost.
        /// </summary>
        public decimal TotalCost { get; set; }

        /// <summary>
        /// Gets or sets the discount.
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// Gets or sets the third party rate code.
        /// </summary>
        public string RateCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether [non refundable].
        /// </summary>
        public bool NonRefundable { get; set; }

        /// <summary>
        /// Gets or sets the cancellation terms.
        /// </summary>
        public List<CancellationTerm> CancellationTerms { get; set; } = new List<CancellationTerm>();

        /// <summary>
        /// Gets or sets the adjustments.
        /// </summary>
        public List<Adjustment> Adjustments { get; set; } = new List<Adjustment>();

        /// <summary>
        /// Gets or sets the commission percentage.
        /// </summary>
        public decimal CommissionPercentage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [on request].
        /// </summary>
        public bool OnRequest { get; set; }

        /// <summary>
        /// Gets or sets the gross cost.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public decimal GrossCost { get; set; }

        /// <summary>
        /// Gets or sets the selling price.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public decimal SellingPrice { get; set; }

        /// <summary>
        /// Gets or sets the rate basis.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string RateBasis { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the special offer.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string SpecialOffer { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the minimum selling price.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public decimal MinimumSellingPrice { get; set; }
    }
}