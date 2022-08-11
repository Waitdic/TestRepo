namespace iVectorOne.Models
{
    /// <summary>
    /// Represents a currency in the Database
    /// </summary>
    public class Currency
    {
        /// <summary>
        /// Gets or sets the International Standard currency identifier.
        /// </summary>
        public int ISOCurrencyID { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the third party currency code.
        /// </summary>
        public string ThirdPartyCurrencyCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        public string CurrencyCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the currency exchange rate.
        /// </summary>
        public decimal ExchangeRate { get; set; } = 1;
    }
}