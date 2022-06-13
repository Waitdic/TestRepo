namespace ThirdParty.Models.Property.Booking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive.Helpers.Extensions;
    using ThirdParty.Models.Property.VirtualCreditCards;
    using ThirdParty.Search.Settings;

    /// <summary>
    /// The property details passed into book and pre books
    /// </summary>
    /// <seealso cref="IThirdPartyAttributeSearch" />
    public class  PropertyDetails : IThirdPartyAttributeSearch
    {
        /// <summary>
        /// Gets or sets The property identifier
        /// </summary>
        public int PropertyID { get; set; }

        /// <summary>
        /// Gets or sets The property identifier
        /// </summary>
        public int TPPropertyID { get; set; }

        /// <summary>
        /// Gets or sets the third party key.
        /// </summary>
        /// <value>
        /// The third party key.
        /// </value>
        public string TPKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The master identifier
        /// </summary>
        public string MasterID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The property name
        /// </summary>
        public string PropertyName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The departure date
        /// </summary>
        public DateTime DepartureDate { get; set; }

        /// <summary>
        /// Gets or sets The arrival date
        /// </summary>
        public DateTime ArrivalDate { get; set; }

        /// <summary>
        /// Gets or sets The language identifier
        /// </summary>
        public int LanguageID { get; set; }

        /// <summary>
        /// Gets or sets The ISO currency code
        /// </summary>
        public string CurrencyCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The currency identifier
        /// </summary>
        public int CurrencyID { get; set; }

        /// <summary>
        /// Gets or sets The selling currency
        /// </summary>
        public string SellingCurrency { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The selling currency exchange rate
        /// </summary>
        public decimal SellingCurrencyExchangeRate { get; set; }

        /// <summary>
        /// Gets or sets The gross cost
        /// </summary>
        public decimal GrossCost { get; set; }

        /// <summary>
        /// Gets or sets The local cost
        /// </summary>
        public decimal LocalCost { get; set; }

        /// <summary>
        /// Gets or sets The total cost
        /// </summary>
        public decimal TotalCost { get; set; }

        /// <summary>
        /// Gets or sets The total price
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Gets or sets The total commission
        /// </summary>
        public decimal TotalCommission { get; set; }

        /// <summary>
        /// Gets or sets The commission percentage
        /// </summary>
        public decimal CommissionPercentage { get; set; }

        /// <summary>
        /// Gets or sets The override rate basis
        /// </summary>
        public string OverrideRateBasis { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The booking comments
        /// </summary>
        public BookingComments BookingComments { get; set; } = new BookingComments();

        /// <summary>
        /// Gets or sets The cancellations
        /// </summary>
        public Cancellations Cancellations { get; set; } = new Cancellations();

        /// <summary>
        /// Gets or sets The errata
        /// </summary>
        public Errata Errata { get; set; } = new Errata();

        /// <summary>
        /// Gets or sets The logs
        /// </summary>
        public Logs Logs { get; set; } = new Logs();

        /// <summary>
        /// Gets or sets The warnings
        /// </summary>
        public Warnings Warnings { get; set; } = new Warnings();

        /// <summary>
        /// Gets or sets The rooms
        /// </summary>
        public List<RoomDetails> Rooms { get; set; } = new List<RoomDetails>();

        ////public Tasks Tasks = new Tasks();

        /// <summary>
        /// Gets or sets The remarks
        /// </summary>
        public List<string> OSIRemarks { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets The booking identifier
        /// </summary>
        public int BookingID { get; set; } = 0;

        /// <summary>
        /// Gets or sets The property booking identifier
        /// </summary>
        public int PropertyBookingID { get; set; } = 0;

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

        /// <summary>
        /// Gets or sets The trade reference
        /// </summary>
        public string TradeReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The third party booking call GUI ds
        /// </summary>
        public List<string> ThirdPartyBookingCallGUIDs { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets The override payment reference
        /// </summary>
        public string OverridePaymentReference { get; set; } = string.Empty;

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
        public DateTime DateOfBirth { get; set; }

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
        public int LeadGuestBookingCountryID { get; set; }

        /// <summary>
        /// Gets or sets The lead guest booking country
        /// </summary>
        public string LeadGuestBookingCountry { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The lead guest phone
        /// </summary>
        public string LeadGuestPhone { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The lead guest mobile
        /// </summary>
        public string LeadGuestMobile { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The lead guest fax
        /// </summary>
        public string LeadGuestFax { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The lead guest email
        /// </summary>
        public string LeadGuestEmail { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The reservations contact
        /// </summary>
        public string ReservationsContact { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The cancellation reason
        /// </summary>
        public string CancellationReason { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The lead guest residence geography level1 identifier
        /// </summary>
        public int LeadGuestResidenceGeographyLevel1ID { get; set; } = 0;

        /// <summary>
        /// Gets or sets The geography level1 identifier
        /// </summary>
        public int GeographyLevel1ID { get; set; }

        /// <summary>
        /// Gets or sets The geography level2 identifier
        /// </summary>
        public int GeographyLevel2ID { get; set; }

        /// <summary>
        /// Gets or sets The geography level3 identifier
        /// </summary>
        public int GeographyLevel3ID { get; set; }

        /// <summary>
        /// Gets or sets The resort code
        /// </summary>
        public string ResortCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The channel manager
        /// </summary>
        public string ChannelManager { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The latest component reference
        /// </summary>
        public string LatestComponentReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether booking must be completed
        /// </summary>
        public bool MustCompleteBooking { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to generate a v card
        /// </summary>
        public bool GenerateVCard { get; set; } = false;

        /// <summary>
        /// Gets or sets v card
        /// </summary>
        public VirtualCardReturn GeneratedVirtualCard { get; set; }  = new VirtualCardReturn();

        ////public VCardDetails VCardDetails = new VCardDetails();
        ////public CardDetails CreditCardDetails;
        ////public ThirdPartyInterfaces.VirtualCreditCardTransaction.VirtualCardReturn GeneratedVirtualCard = new ThirdPartyInterfaces.VirtualCreditCardTransaction.VirtualCardReturn();

        /// <summary>
        /// Gets or sets a value indicating whether its on request
        /// </summary>
        public bool OnRequest { get; set; } = false;

        /// <summary>
        /// Gets or sets the profile XML.
        /// </summary>
        /// <value>
        /// The profile XML.
        /// </value>
        public string ProfileXML { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether pay local is available
        /// </summary>
        public bool PayLocalAvailable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether pay local is required
        /// </summary>
        public bool PayLocalRequired { get; set; }

        /// <summary>
        /// Get or sets the unique nationality code 
        /// </summary>
        /// <value>
        ///  The nationality code
        /// </value>
        public string NationalityCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the boolean to decide whether opaque rates are supported.
        /// </summary>
        /// <value>
        /// The SupportOpaqueRates boolean.
        /// </value>
        public bool OpaqueRates { get; set; }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public int Duration
        {
            get
            {
                return (this.DepartureDate - this.ArrivalDate).TotalDays.ToSafeInt();
            }
        }

        /// <summary>
        /// Gets the adults.
        /// </summary>
        /// <value>
        /// The adults.
        /// </value>
        public int Adults
        {
            get
            {
                return this.Rooms.Sum(o => o.Adults);
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public int Children
        {
            get
            {
                return this.Rooms.Sum(o => o.Children);
            }
        }

        /// <summary>
        /// Gets the infants.
        /// </summary>
        /// <value>
        /// The infants.
        /// </value>
        public int Infants
        {
            get
            {
                return this.Rooms.Sum(o => o.Infants);
            }
        }

        //// TODO when you plug this into ivector you'll need to look this up from TP data
        //// Might be easier to not add a depenency in here, and set it based on tp when you new up the property details
        /// <summary>
        /// Gets a value indicating whether [create logs].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [create logs]; otherwise, <c>false</c>.
        /// </value>
        public bool CreateLogs { get; set; } = false; 

        ////public List<TPInstallment> Installments { get; set; } = new List<TPInstallment>();

        /// <summary>
        /// Gets or sets the selling country.
        /// </summary>
        /// <value>
        /// The ISO 3166-2 country code.
        /// </value>
        public string SellingCountry { get; set; } = string.Empty;

        public List<ThirdPartyConfiguration> ThirdPartyConfigurations { get; set; }

        /// <summary>
        /// Adds the log.
        /// </summary>
        /// <param name="title">The s title.</param>
        /// <param name="text">The s text.</param>
        public void AddLog(string title, string text)
        {
            Logs.AddNew(this.Source, title, text);
        }
    }
}