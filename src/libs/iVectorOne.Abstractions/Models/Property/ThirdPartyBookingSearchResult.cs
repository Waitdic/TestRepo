namespace iVectorOne.Models.Property
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A return object for third party booking searches
    /// </summary>
    public partial class ThirdPartyBookingSearchResult
    {
        /// <summary>
        /// Gets or sets The supplier reference identifier
        /// </summary>
        public string SupplierReferenceID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The guest name
        /// </summary>
        public string GuestName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The local cost
        /// </summary>
        public decimal LocalCost { get; set; }

        /// <summary>
        /// Gets or sets The currency code
        /// </summary>
        public string CurrencyCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The booking status
        /// </summary>
        public Status BookingStatus { get; set; }

        /// <summary>
        /// Gets or sets The booking date
        /// </summary>
        public DateTime BookingDate { get; set; }

        /// <summary>
        /// Gets or sets The arrival date
        /// </summary>
        public DateTime ArrivalDate { get; set; }

        /// <summary>
        /// Gets or sets The duration
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets The components
        /// </summary>
        public List<ComponentDetails> Components { get; set; } = new List<ComponentDetails>();
    }
}
