namespace iVectorOne.Models
{
    /// <summary>
    /// Settings used to configure the search
    /// </summary>
    public class Settings
    {
        /// <summary>Gets or sets the property third party request limit.</summary>
        public int PropertyTPRequestLimit { get; set; }

        /// <summary>Gets or sets the search timeout seconds.</summary>
        public int SearchTimeoutSeconds { get; set; } = 60;

        /// <summary>Gets or sets the ISO currency code.</summary>
        public string CurrencyCode { get; set; } = string.Empty;

        /// <summary>Gets or sets the single room de-duplicating algorithm.</summary>
        public DedupeMethod SingleRoomDedupingAlgorithm { get; set; } = DedupeMethod.CheapestLeadin;

        /// <summary>Gets or sets a value indicating whether [de-duplicated by non refundable].</summary>
        public bool DedupeByNonRefundable { get; set; }

        /// <summary>Gets or sets a value indicating whether [unknown non refundable as refundable].</summary>
        public bool UnknownNonRefundableAsRefundable { get; set; }
    }
}