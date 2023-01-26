namespace iVectorOne.Models.Property
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A booking search details object
    /// </summary>
    /// <seealso cref="ThirdParty.IThirdPartyAttributeSearch" />
    public class BookingSearchDetails : IThirdPartyAttributeSearch
    {
        /// <summary>
        /// Gets or sets the source reference.
        /// </summary>
        public string SourceReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first name of the lead guest.
        /// </summary>
        public string LeadGuestFirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name of the lead guest.
        /// </summary>
        public string LeadGuestLastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the lead guest nationality identifier.
        /// </summary>
        public int LeadGuestNationalityID { get; set; } = 0;

        /// <summary>
        /// Gets or sets the booking status.
        /// </summary>
        public string BookingStatus { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the booking date from.
        /// </summary>
        public DateTime BookingDateFrom { get; set; }

        /// <summary>
        /// Gets or sets the booking date to.
        /// </summary>
        public DateTime BookingDateTo { get; set; }

        /// <summary>
        /// Gets or sets the arrival date from.
        /// </summary>
        public DateTime ArrivalDateFrom { get; set; }

        /// <summary>
        /// Gets or sets the arrival date to.
        /// </summary>
        public DateTime ArrivalDateTo { get; set; }

        /// <summary>
        /// Gets or sets the resort code.
        /// </summary>
        public string ResortCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the departure airport.
        /// </summary>
        public string DepartureAirport { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the departure airport group.
        /// </summary>
        public string DepartureAirportGroup { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the arrival airport.
        /// </summary>
        public string ArrivalAirport { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer reference.
        /// </summary>
        public string CustomerReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the language identifier.
        /// </summary>
        public int LanguageID { get; set; }

        /// <summary>
        /// Gets or sets The ISO currency code
        /// </summary>
        public string CurrencyCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the third party configurations
        /// </summary>
        public List<ThirdPartyConfiguration> ThirdPartyConfigurations { get; set; } = new();
    }
}