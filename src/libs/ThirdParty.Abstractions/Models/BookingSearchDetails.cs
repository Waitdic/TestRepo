namespace ThirdParty.Models
{
    using System;
    using System.Collections.Generic;
    using ThirdParty.Search.Settings;

    /// <summary>
    /// A booking search details object
    /// </summary>
    /// <seealso cref="ThirdParty.IThirdPartyAttributeSearch" />
    public class BookingSearchDetails : IThirdPartyAttributeSearch
    {
        /// <summary>
        /// Gets or sets the source reference.
        /// </summary>
        /// <value>
        /// The source reference.
        /// </value>
        public string SourceReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first name of the lead guest.
        /// </summary>
        /// <value>
        /// The first name of the lead guest.
        /// </value>
        public string LeadGuestFirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name of the lead guest.
        /// </summary>
        /// <value>
        /// The last name of the lead guest.
        /// </value>
        public string LeadGuestLastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the lead guest nationality identifier.
        /// </summary>
        /// <value>
        /// The lead guest nationality identifier.
        /// </value>
        public int LeadGuestNationalityID { get; set; } = 0;

        /// <summary>
        /// Gets or sets the booking status.
        /// </summary>
        /// <value>
        /// The booking status.
        /// </value>
        public string BookingStatus { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the booking date from.
        /// </summary>
        /// <value>
        /// The booking date from.
        /// </value>
        public DateTime BookingDateFrom { get; set; }

        /// <summary>
        /// Gets or sets the booking date to.
        /// </summary>
        /// <value>
        /// The booking date to.
        /// </value>
        public DateTime BookingDateTo { get; set; }

        /// <summary>
        /// Gets or sets the arrival date from.
        /// </summary>
        /// <value>
        /// The arrival date from.
        /// </value>
        public DateTime ArrivalDateFrom { get; set; }

        /// <summary>
        /// Gets or sets the arrival date to.
        /// </summary>
        /// <value>
        /// The arrival date to.
        /// </value>
        public DateTime ArrivalDateTo { get; set; }

        /// <summary>
        /// Gets or sets the resort code.
        /// </summary>
        /// <value>
        /// The resort code.
        /// </value>
        public string ResortCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the departure airport.
        /// </summary>
        /// <value>
        /// The departure airport.
        /// </value>
        public string DepartureAirport { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the departure airport group.
        /// </summary>
        /// <value>
        /// The departure airport group.
        /// </value>
        public string DepartureAirportGroup { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the arrival airport.
        /// </summary>
        /// <value>
        /// The arrival airport.
        /// </value>
        public string ArrivalAirport { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer reference.
        /// </summary>
        /// <value>
        /// The customer reference.
        /// </value>
        public string CustomerReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the language identifier.
        /// </summary>
        /// <value>
        /// The language identifier.
        /// </value>
        public int LanguageID { get; set; }

        /// <summary>
        /// Gets or sets The ISO currency code
        /// </summary>
        public string CurrencyCode { get; set; } = string.Empty;

        public List<ThirdPartyConfiguration> ThirdPartyConfigurations { get; set; }
    }
}