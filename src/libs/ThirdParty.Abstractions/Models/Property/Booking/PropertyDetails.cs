namespace ThirdParty.Models.Property.Booking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using ThirdParty.Models.Property.VirtualCreditCards;
    using ThirdParty.Search.Settings;

    /// <summary>
    /// The property details passed into book and pre books
    /// </summary>
    /// <seealso cref="IThirdPartyAttributeSearch" />
    public class  PropertyDetails : IThirdPartyAttributeSearch
    {
        /// <summary>
        /// Gets or sets the subscription identifier
        /// </summary>
        public int SubscriptionID { get; set; }

        /// <summary>
        /// Gets or sets The property identifier
        /// </summary>
        public int CentralPropertyID { get; set; }

        /// <summary>
        /// Gets or sets The property identifier
        /// </summary>
        public int PropertyID { get; set; }

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        public string PropertyName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the third party key.
        /// </summary>
        public string TPKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The departure date
        /// </summary>
        public DateTime DepartureDate { get; set; }

        /// <summary>
        /// Gets or sets The arrival date
        /// </summary>
        public DateTime ArrivalDate { get; set; }

        // todo - replace with TP currency code as this is what the interfaces are expecting, currently set to ISO code
        /// <summary>
        /// Gets or sets The ISO currency code
        /// </summary>
        public string CurrencyCode { get; set; } = string.Empty;

        // todo - remove as never set, replace with currency code
        /// <summary>
        /// Gets or sets The currency identifier
        /// </summary>
        public int CurrencyID { get; set; }

        /// <summary>
        /// Gets or sets The gross cost
        /// </summary>
        public decimal GrossCost { get; set; }

        /// <summary>
        /// Gets or sets The local cost
        /// </summary>
        public decimal LocalCost { get; set; }

        /// <summary>
        /// Gets or sets The booking comments
        /// </summary>
        public BookingComments BookingComments { get; set; } = new();

        /// <summary>
        /// Gets or sets The cancellations
        /// </summary>
        public Cancellations Cancellations { get; set; } = new();

        /// <summary>
        /// Gets or sets The errata
        /// </summary>
        public Errata Errata { get; set; } = new();

        /// <summary>
        /// Gets or sets The logs
        /// </summary>
        public Logs Logs { get; set; } = new();

        /// <summary>
        /// Gets or sets The warnings
        /// </summary>
        public Warnings Warnings { get; set; } = new();

        /// <summary>
        /// Gets or sets The rooms
        /// </summary>
        public List<RoomDetails> Rooms { get; set; } = new();

        // todo - probably remove
        /// <summary>
        /// Gets or sets The booking identifier
        /// </summary>
        public int BookingID { get; set; } = 0;

        /// <summary>
        /// Gets or sets The booking reference
        /// </summary>
        public string BookingReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The source
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The source reference
        /// </summary>
        public string SourceReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The source secondary reference
        /// </summary>
        public string SourceSecondaryReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The supplier source reference
        /// </summary>
        public string SupplierSourceReference { get; set; } = string.Empty;

        // todo - possibly remove, as never set
        /// <summary>
        /// Gets or sets The supplier information
        /// </summary>
        public string SupplierInfo { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The Third party ref1
        /// </summary>
        public string TPRef1 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The Third party ref2
        /// </summary>
        public string TPRef2 { get; set; } = string.Empty;

        // todo - remove as never set
        /// <summary>
        /// Gets or sets The trade reference
        /// </summary>
        public string TradeReference { get; set; } = string.Empty;

        // todo - remove as never used
        /// <summary>
        /// Gets or sets The cancellation amount
        /// </summary>
        public decimal CancellationAmount { get; set; }

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

        // todo - remove or set in the factory
        /// <summary>
        /// Gets or sets  The passport number
        /// </summary>
        public string PassportNumber { get; set; } = string.Empty;

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
        /// Gets or sets The resort code
        /// </summary>
        public string ResortCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The channel manager
        /// </summary>
        public string ChannelManager { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets v card
        /// </summary>
        public VirtualCardReturn GeneratedVirtualCard { get; set; }  = new VirtualCardReturn();

        /// <summary>
        /// Get or sets the unique nationality code 
        /// </summary>
        public string NationalityCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the boolean to decide whether opaque rates are supported.
        /// </summary>
        public bool OpaqueRates { get; set; }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        public int Duration => (this.DepartureDate - this.ArrivalDate).TotalDays.ToSafeInt();

        /// <summary>
        /// Gets the adults.
        /// </summary>
        public int Adults => this.Rooms.Sum(o => o.Adults);

        /// <summary>
        /// Gets the children.
        /// </summary>
        public int Children => this.Rooms.Sum(o => o.Children);

        /// <summary>
        /// Gets the infants.
        /// </summary>
        public int Infants => this.Rooms.Sum(o => o.Infants);

        /// <summary>
        /// Gets or sets the selling country.
        /// </summary>
        public string SellingCountry { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the 
        /// </summary>
        public List<ThirdPartyConfiguration> ThirdPartyConfigurations { get; set; } = new();

        /// <summary>
        /// Adds the log.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="request">The web request.</param>
        public void AddLog(string title, Request request)
        {
            Logs.AddNew(this.Source, title, request.RequestLog, request.ResponseLog);
        }
    }
}