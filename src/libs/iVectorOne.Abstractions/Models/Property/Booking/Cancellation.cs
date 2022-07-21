namespace iVectorOne.Models.Property.Booking
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// A cancellation
    /// </summary>
    public class Cancellation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cancellation"/> class.
        /// </summary>
        public Cancellation()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cancellation"/> class.
        /// </summary>
        /// <param name="startDate">The d start date.</param>
        /// <param name="endDate">The d end date.</param>
        /// <param name="amount">The n amount.</param>
        public Cancellation(DateTime startDate, DateTime endDate, decimal amount)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Amount = amount;
        }

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