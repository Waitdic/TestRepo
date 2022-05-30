namespace ThirdParty.Search.Settings
{
    using ThirdParty.Results.Models;

    /// <summary>
    /// Settings used to configure the search
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Gets or sets the property third party request limit.
        /// </summary>
        /// <value>
        /// The property third party request limit.
        /// </value>
        public int PropertyTPRequestLimit { get; set; }

        /// <summary>Gets or sets the search timeout seconds.</summary>
        /// <value>The search timeout seconds.</value>
        public int SearchTimeoutSeconds { get; set; } = 60;

        /// <summary>Gets or sets a value indicating whether [log main search error].</summary>
        /// <value>
        /// <c>true</c> if [log main search error]; otherwise, <c>false</c>.</value>
        public bool LogMainSearchError { get; set; } = false;

        /// <summary>Gets or sets the selling currency identifier.</summary>
        /// <value>The selling currency identifier.</value>
        public int SellingCurrencyID { get; set; } = 0;

        /// <summary>Gets or sets the ISO currency code.</summary>
        /// <value>The ISO currency code.</value>
        public string CurrencyCode { get; set; } = string.Empty;

        /// <summary>Gets or sets the single room de-duplicating algorithm.</summary>
        /// <value>The single room de-duplicating algorithm.</value>
        public DedupeMethod SingleRoomDedupingAlgorithm { get; set; } = DedupeMethod.CheapestLeadin;

        /// <summary>Gets or sets a value indicating whether [de-duplicated by non refundable].</summary>
        /// <value>
        /// <c>true</c> if [de-duplicate by non refundable]; otherwise, <c>false</c>.</value>
        public bool DedupeByNonRefundable { get; set; } = false;

        /// <summary>Gets or sets a value indicating whether [unknown non refundable as refundable].</summary>
        /// <value>
        ///   <c>true</c> if [unknown non refundable as refundable]; otherwise, <c>false</c>.</value>
        public bool UnknownNonRefundableAsRefundable { get; set; } = false;

        /// <summary>Gets or sets a value indicating whether [search costs exhaust enabled].</summary>
        /// <value>
        /// <c>true</c> if [search costs exhaust enabled]; otherwise, <c>false</c>.</value>
        public bool SearchCostsExhaustEnabled { get; set; } = false;

        /// <summary>Gets or sets the search costs exhaust restricted third parties CSV.</summary>
        /// <value>The search costs exhaust restricted third parties CSV.</value>
        public string SearchCostsExhaustRestrictedThirdPartiesCSV { get; set; } = string.Empty;

        /// <summary>Gets or sets the search costs exhaust file dump folder.</summary>
        /// <value>The search costs exhaust file dump folder.</value>
        public string SearchCostsExhaustFileDumpFolder { get; set; } = string.Empty;

        /// <summary>Gets or sets a value indicating whether [use override third parties].</summary>
        /// <value>
        /// <c>true</c> if [use override third parties]; otherwise, <c>false</c>.</value>
        public bool UseOverrideThirdParties { get; set; } = false;
    }
}
