namespace ThirdParty.Results.Models
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// cancellation terms
    /// </summary>
    public class Cancellation
    {
        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>
        /// The start date.
        /// </value>
        [XmlAttribute("SD")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>
        /// The end date.
        /// </value>
        [XmlAttribute("ED")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        [XmlAttribute("AMT")]
        public decimal Amount { get; set; }
    }
}