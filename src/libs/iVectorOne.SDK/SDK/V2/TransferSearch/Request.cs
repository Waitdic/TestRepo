﻿namespace iVectorOne.SDK.V2.TransferSearch
{
    using MediatR;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A transfer search request
    /// </summary>
    public record Request : RequestBase, IRequest<Response>
    {
        /// <summary>
        /// Gets or sets the departure date.
        /// </summary>
        /// <value>
        /// The departure date.
        /// </value>
        public DateTime? DepartureDate { get; set; }

        /// <summary>
        /// Gets or sets the departure time.
        /// </summary>
        /// <value>
        /// The departure time.
        /// </value>
        public string DepartureTime { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the departure location identifier.
        /// </summary>
        /// <value>
        /// The departure location.
        /// </value>
        public int DepartureLocationID { get; set; }

        /// <summary>
        /// Gets or sets the arrival location identifier.
        /// </summary>
        /// <value>
        /// The arrival location.
        /// </value>
        public int ArrivalLocationID { get; set; }

        /// <summary>
        /// Gets or sets the adults.
        /// </summary>
        /// <value>
        /// The adults.
        /// </value>
        public int Adults { get; set; }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public int Children { get; set; }

        /// <summary>
        /// Gets or sets the infants.
        /// </summary>
        /// <value>
        /// The infants.
        /// </value>
        public int Infants { get; set; }

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        /// <value>
        /// The currency code.
        /// </value>
        public string CurrencyCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the selling country.
        /// </summary>
        /// <value>
        /// The ISO 3166-2 country code.
        /// </value>
        public string SellingCountry { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the supplier.
        /// </summary>
        /// <value>
        /// The supplier
        /// </value>
        public string Supplier { get; set; } = String.Empty;

        public string EmailLogsToAddress { get; set; } = string.Empty;
    }
}
