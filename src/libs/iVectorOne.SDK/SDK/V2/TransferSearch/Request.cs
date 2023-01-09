namespace iVectorOne.SDK.V2.TransferSearch
{
    using MediatR;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A transfer search request
    /// </summary>
    public record Request : TransferRequestBase, IRequest<Response>
    {
        /// <summary>
        /// Gets or sets the departure location identifier.
        /// </summary>
        /// <value>
        /// The departure location.
        /// </value>DepartureL
        public int DepartureLocationID { get; set; }

        /// <summary>
        /// Gets or sets the arrival location identifier.
        /// </summary>
        /// <value>
        /// The arrival location.
        /// </value>
        public int ArrivalLocationID { get; set; }

        /// <summary>
        /// Gets or sets the additional departure location identifiers.
        /// </summary>
        /// <value>
        /// Additional departure locations.
        /// </value>
        public List<int> AdditionalDepartureLocationIDs { get; set; } = new List<int>();

        /// <summary>
        /// Gets or sets the additional arrival location identifiers.
        /// </summary>
        /// <value>
        /// Additional arrival locations.
        /// </value>
        public List<int> AdditionalArrivalLocationIDs { get; set; } = new List<int>();
        
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
        /// Gets or sets the one way flag.
        /// </summary>
        /// <value>
        /// The one way flag.
        /// </value>
        public bool OneWay { get; set; }

        /// <summary>
        /// Gets or sets the return date.
        /// </summary>
        /// <value>
        /// The return date.
        /// </value>
        public DateTime? ReturnDate { get; set; }

        /// <summary>
        /// Gets or sets the return time.
        /// </summary>
        /// <value>
        /// The return time.
        /// </value>
        public string ReturnTime { get; set; } = string.Empty;

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
        /// Gets or sets the child ages.
        /// </summary>
        public List<int> ChildAges { get; set; } = new List<int>();

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
        //public string SellingCountry { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the supplier.
        /// </summary>
        /// <value>
        /// The supplier
        /// </value>
        public string Supplier { get; set; } = String.Empty;

        public string EmailLogsToAddress { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets a value indicating whether to include on request transfers in the results
        /// </summary>
        public bool IncludeOnRequest { get; set; } = false;
    }
}
