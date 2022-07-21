namespace iVectorOne.Models
{
    /// <summary>
    /// Represents a currency in the Database
    /// </summary>
    public class Currency
    {
        /// <summary>
        /// Gets or sets the currency identifier.
        /// </summary>
        /// <value>
        /// The currency identifier.
        /// </value>
        public int CurrencyID { get; set; }

        /// <summary>
        /// Gets or sets the International Standard currency identifier.
        /// </summary>
        /// <value>
        /// The International Standard currency identifier.
        /// </value>
        public int ISOCurrencyID { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the third party currency code.
        /// </summary>
        /// <value>
        /// The third party currency code.
        /// </value>
        public string ThirdPartyCurrencyCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        /// <value>
        /// The currency code.
        /// </value>
        public string CurrencyCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the currency exchange rate.
        /// </summary>
        /// <value>
        /// The exchange rate.
        /// </value>
        public decimal ExchangeRate { get; set; } = 1;
    }
}
