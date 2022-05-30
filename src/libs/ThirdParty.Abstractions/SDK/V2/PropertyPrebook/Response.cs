﻿namespace ThirdParty.SDK.V2.PropertyPrebook
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using ThirdParty.SDK.V2.PropertySearch;

    public record Response
    {
        /// <summary>
        /// Gets or sets the booking token.
        /// </summary>
        public string BookingToken { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier reference1.</summary>
        public string SupplierReference1 { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier reference2.</summary>
        public string SupplierReference2 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total cost.
        /// </summary>
        public decimal TotalCost { get; set; }

        /// <summary>
        /// Gets or sets the room bookings.
        /// </summary>
        public List<RoomBooking> RoomBookings { get; set; } = new();

        /// <summary>
        /// Gets or sets the cancellation terms.
        /// </summary>
        public List<CancellationTerm> CancellationTerms { get; set; } = new();

        /// <summary>
        /// Gets or sets the errata.
        /// </summary>
        public List<string> Errata { get; set; } = new List<string>();

        /// <summary>Any warnings raised on the pre book response</summary>
        /// <value>The warnings.</value>
        [JsonIgnore]
        public List<string> Warnings { get; set; } = new List<string>();
    }
}