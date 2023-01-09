﻿
namespace iVectorOne.Models.Extra
{
    using System;
    using System.Collections.Generic;
    using Intuitive.Helpers.Net;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Models.SupplierLog;
    using ExtraBook = iVectorOne.SDK.V2.ExtraBook;

    /// <summary>
    /// The extra details passed into book and pre books
    /// </summary>
    /// <seealso cref="IThirdPartyAttributeSearch" />
    public class ExtraDetails : IThirdPartyAttributeSearch
    {
        /// <summary>
        /// Gets or sets The booking reference
        /// </summary>
        public string BookingReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The booking identifier
        /// </summary>
        public int TransferBookingID { get; set; }

        /// <summary>
        /// Gets or sets the account identifier
        /// </summary>
        public int AccountID { get; set; }

        /// <summary>
        /// Gets or sets the departure date.
        /// </summary>
        /// <value>
        /// The departure date.
        /// </value>
        public DateTime DepartureDate { get; set; }

        /// <summary>
        /// Gets or sets The departure time
        /// </summary>
        public string DepartureTime { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the transfer is one way
        /// </summary>
        public bool OneWay { get; set; }

        /// <summary>
        /// Gets or sets The return date
        /// </summary>
        public DateTime ReturnDate { get; set; }

        /// <summary>
        /// Gets or sets The return time
        /// </summary>
        public string ReturnTime { get; set; } = string.Empty;

        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <value>
        /// The duration. Zero for one way
        /// </value>
        public int Duration => OneWay ? 0 : (ReturnDate - DepartureDate).Days;

        /// <summary>
        /// Gets or sets The ISO currency code
        /// </summary>
        public string ISOCurrencyCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The gross cost
        /// </summary>
        public decimal GrossCost { get; set; }

        /// <summary>
        /// Gets or sets The local cost
        /// </summary>
        public decimal LocalCost { get; set; }

        /// <summary>
        /// Gets or sets The cancellations
        /// </summary>
        public Cancellations Cancellations { get; set; } = new();

        /// <summary>
        /// Gets or sets The logs
        /// </summary>
        public Logs Logs { get; set; } = new();

        /// <summary>
        /// Gets or sets The supplier logs
        /// </summary>
        public List<SupplierLog> SupplierLogs { get; set; } = new();

        /// <summary>
        /// Gets or sets The warnings
        /// </summary>
        public Warnings Warnings { get; set; } = new();

        /// <summary> Gets or sets The passengers</summary>
        public Passengers Passengers { get; set; } = new();

        /// <summary>
        /// Gets or sets The source
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The supplier identifier
        /// </summary>
        public int SupplierID { get; set; }

        ///// <summary>
        ///// Gets or sets The supplier reference
        ///// </summary>
        public string SupplierReference { get; set; } = string.Empty;

        ///// <summary>
        ///// Gets or sets The source reference
        ///// </summary>
        //public string SourceReference { get; set; } = string.Empty;

        ///// <summary>
        ///// Gets or sets The confirmation reference
        ///// </summary>
        public string ConfirmationReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets the adults.
        /// </summary>
        public int Adults { get; set; }

        /// <summary>
        /// Gets the children.
        /// </summary>
        public int Children { get; set; }

        /// <summary>
        /// Gets the infants.
        /// </summary>
        public int Infants { get; set; }

        /// <summary>
        /// Gets or sets The lead guest title
        /// </summary>
        public string LeadGuestTitle { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The lead guest first name
        /// </summary>
        public string LeadGuestFirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The lead guest last name
        /// </summary>
        public string LeadGuestLastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The date of birth
        /// </summary>
        public DateTime LeadGuestDateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets  The passport number
        /// </summary>
        public string LeadGuestPassportNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The lead guest address1
        /// </summary>
        public string LeadGuestAddress1 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The lead guest address2
        /// </summary>
        public string LeadGuestAddress2 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The lead guest town city
        /// </summary>
        public string LeadGuestTownCity { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The lead guest county
        /// </summary>
        public string LeadGuestCounty { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The lead guest postcode
        /// </summary>
        public string LeadGuestPostcode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The lead guest booking country identifier
        /// </summary>
        public string LeadGuestCountryCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The lead guest phone
        /// </summary>
        public string LeadGuestPhone { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The lead guest mobile
        /// </summary>
        public string LeadGuestMobile { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The lead guest email
        /// </summary>
        public string LeadGuestEmail { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the third party configurations
        /// </summary>
        public List<ThirdPartyConfiguration> ThirdPartyConfigurations { get; set; } = new();

        /// <summary>
        /// Gets or sets The errata
        /// </summary>
        public Errata DepartureErrata { get; set; } = new();

        /// <summary>
        /// Gets or sets The errata
        /// </summary>
        public Errata ReturnErrata { get; set; } = new();

        ///// <summary>
        ///// Gets or sets the outbound details
        ///// </summary>
        public ExtraBook.JourneyDetails OutboundDetails { get; set; } = new();

        ///// <summary>
        ///// Gets or sets the return details
        ///// </summary>
        public ExtraBook.JourneyDetails ReturnDetails { get; set; } = new();

        ///// <summary>
        ///// Gets or sets The arrival date
        ///// </summary>
        //public DateTime ArrivalDate { get; set; }


        ///// <summary>
        ///// Gets the duration.
        ///// </summary>
        //public int Duration => (this.DepartureDate - this.ArrivalDate).TotalDays.ToSafeInt();


        ///// <summary>
        ///// Gets or sets the selling country.
        ///// </summary>
        //public string SellingCountry { get; set; } = string.Empty;

        ///// <summary>
        ///// Gets or sets the booking identifier
        ///// </summary>
        //public int BookingID { get; set; }

        /// <summary>
        /// Gets or sets the third party settings
        /// </summary>
        public Dictionary<string, string> ThirdPartySettings { get; set; } = new Dictionary<string, string> { };

        /// <summary>
        /// Adds the log.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="request">The web request.</param>
        public void AddLog(string title, Request request)
        {
            // todo - remove legacy log collection (merge with supplier logs)
            Logs.AddNew(this.Source, title, request.RequestLog, request.ResponseLog);
            SupplierLogs.Add(new SupplierLog()
            {
                Title = title,
                Request = request,
            });
        }
    }
}