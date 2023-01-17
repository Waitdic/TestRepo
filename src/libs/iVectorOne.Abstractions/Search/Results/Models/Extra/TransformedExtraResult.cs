using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace iVectorOne.Search.Results.Models.Extra
{
    /// <summary>
    /// The transformed extra results
    /// </summary>
    [XmlType("Result")]
    public class TransformedExtraResult
    {
        /// <summary>
        /// The extra name error message
        /// </summary>
        [XmlIgnore]
        public const string EXTRANAMEERRORMESSAGE = "No extra name Specified";

        /// <summary>
        /// The currency code error message
        /// </summary>
        [XmlIgnore]
        public const string CURRENCYCODEERRORMESSAGE = "No Currency Code Specified";

        /// <summary>
        /// The cost error message
        /// </summary>
        [XmlIgnore]
        public const string COSTERRORMESSAGE = "No Valid Cost Specified";

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        [XmlIgnore]
        public List<string> Warnings { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the tp session id.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public string TPSessionID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the supplier reference.
        /// </summary>
        /// <value>
        /// The extra vehicle.
        /// </value>
        public string SupplierReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        /// <value>
        /// The extra currency code.
        /// </value>
        public string CurrencyCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the cost.
        /// </summary>
       public decimal Cost { get; set; }

        /// <summary>
        /// Gets or sets the extra name.
        /// </summary>
        public string ExtraName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the extra category.
        /// </summary>
        /// <value>
        /// The extra category.
        /// </value>
        public string ExtraCategory { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the use date.
        /// </summary>
        /// <value>
        /// The use date.
        /// </value>
        public string UseDate { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the end date
        /// </summary>
        public string EndDate { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the use date
        /// </summary>
        public string UseTime { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the end time
        /// </summary>
        public string EndTime { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the additional details.
        /// </summary>
        public string AdditionalDetails { get; set; } = string.Empty;

        /// <summary>
        /// Validates this instance.
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrEmpty(this.CurrencyCode))
            {
                this.Warnings.Add(CURRENCYCODEERRORMESSAGE);
            }

            if (string.IsNullOrEmpty(this.ExtraName))
            {
                this.Warnings.Add(ExtraName);
            }

            if (this.Cost == 0)
            {
                this.Warnings.Add(COSTERRORMESSAGE);
            }
        }
    }
}
