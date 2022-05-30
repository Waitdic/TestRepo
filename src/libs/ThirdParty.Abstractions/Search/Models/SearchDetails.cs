namespace ThirdParty.Search.Models
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;
    using Intuitive.Helpers.Extensions;
    using iVector.Pricing.Enums;
    using iVector.Search.Property;
    using ThirdParty.Search.Settings;

    /// <summary>
    /// A search details object
    /// </summary>
    /// <seealso cref="ISearchDetails" />
    /// <seealso cref="IThirdPartyAttributeSearch" />
    public class SearchDetails : ISearchDetails, IThirdPartyAttributeSearch
    {
        /// <summary>
        /// class wouldn't serialize if this was a public property
        /// </summary>
        private System.Collections.Specialized.NameValueCollection queryString = new System.Collections.Specialized.NameValueCollection();

        /// <summary>Gets or sets the settings.</summary>
        /// <value>The settings.</value>
        public Settings Settings { get; set; } = new();

        /// <summary>Gets or sets the third party configurations.</summary>
        /// <value>The third party configurations.</value>
        public List<ThirdPartyConfiguration> ThirdPartyConfigurations { get; set; } = new();

        /// <summary>Gets or sets the type of the logging.</summary>
        /// <value>The type of the logging.</value>
        public string LoggingType { get; set; } = "None";

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>
        /// The session identifier.
        /// </value>
        public string SessionID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the store connect string.
        /// </summary>
        /// <value>
        /// The store connect string.
        /// </value>
        public string StoreConnectString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The search mode
        /// </summary>
        public SearchMode SearchMode { get; set; }

        /// <summary>
        /// Gets or sets the booking source identifier.
        /// </summary>
        /// <value>
        /// The booking source identifier.
        /// </value>
        public int BookingSourceID { get; set; }

        /// <summary>
        /// Gets or sets The source
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The Internet Protocol address
        /// </summary>
        public string IPAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The URL
        /// </summary>
        public string URL { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The email logs to address
        /// </summary>
        public string EmailLogsToAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The email logs to provider
        /// </summary>
        public string EmailLogsToProvider { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The departure parent type
        /// </summary>
        public string DepartureParentType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The departure parent identifier
        /// </summary>
        public int DepartureParentID { get; set; }

        /// <summary>
        /// Gets or sets The arrival parent type
        /// </summary>
        public string ArrivalParentType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The arrival parent identifier
        /// </summary>
        public int ArrivalParentID { get; set; }

        /// <summary>
        /// Gets or sets The flight class identifier
        /// </summary>
        public int FlightClassID { get; set; }

        /// <summary>
        /// Gets or sets The alt return airport identifier
        /// </summary>
        public int AltReturnAirportID { get; set; } = 0;

        /// <summary>
        /// Gets or sets the airport identifier.
        /// </summary>
        /// <value>
        /// The airport identifier.
        /// </value>
        public int AirportID { get; set; }

        /// <summary>
        /// Gets or sets The airport group identifier
        /// </summary>
        public int AirportGroupID { get; set; }

        /// <summary>
        /// Gets or sets the geography level1 identifier.
        /// </summary>
        /// <value>
        /// The geography level1 identifier.
        /// </value>
        public int GeographyLevel1ID { get; set; }

        /// <summary>
        /// Gets or sets the geography level2 identifier.
        /// </summary>
        /// <value>
        /// The geography level2 identifier.
        /// </value>
        public int GeographyLevel2ID { get; set; }

        /// <summary>
        /// Gets or sets the geography level3 identifier.
        /// </summary>
        /// <value>
        /// The geography level3 identifier.
        /// </value>
        public int GeographyLevel3ID { get; set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        /// <value>
        /// The latitude.
        /// </value>
        public decimal Latitude { get; set; } = 0;

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        /// <value>
        /// The longitude.
        /// </value>
        public decimal Longitude { get; set; } = 0;

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <value>
        /// The radius.
        /// </value>
        public decimal Radius { get; set; } = 0;

        /// <summary>
        /// Gets or sets the booking date.
        /// </summary>
        /// <value>
        /// The booking date.
        /// </value>
        public DateTime BookingDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the arrival date.
        /// </summary>
        /// <value>
        /// The arrival date.
        /// </value>
        public DateTime ArrivalDate { get; set; } = DateTimeExtensions.EmptyDate;

        /// <summary>
        /// Gets or sets The override property arrival date
        /// </summary>
        public DateTime OverridePropertyArrivalDate { get; set; } = DateTimeExtensions.EmptyDate;

        /// <summary>
        /// Gets or sets The override property departure date
        /// </summary>
        public DateTime OverridePropertyDepartureDate { get; set; } = DateTimeExtensions.EmptyDate;

        /// <summary>
        /// Gets or sets The offset days
        /// </summary>
        public int OffsetDays { get; set; }

        /// <summary>
        /// Gets or sets The holiday duration type
        /// </summary>
        public HolidayDurationType HolidayDurationType { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public int Duration { get; set; }

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
        /// Gets or sets The return date
        /// </summary>
        public DateTime ReturnDate { get; set; }

        /// <summary>
        /// Gets or sets The return time
        /// </summary>
        public string ReturnTime { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The date flexibility
        /// </summary>
        public int DateFlexibility { get; set; } = 3;

        /// <summary>
        /// Gets or sets The rooms
        /// </summary>
        public int Rooms { get; set; }

        /// <summary>
        /// Gets or sets The property source
        /// </summary>
        public string PropertySource { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the property identifier.
        /// </summary>
        /// <value>
        /// The property identifier.
        /// </value>
        public int PropertyID { get; set; }

        /// <summary>
        /// Gets or sets the property reference.
        /// </summary>
        /// <value>
        /// The property reference.
        /// </value>
        public string PropertyReference { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the property reference identifier.
        /// </summary>
        /// <value>
        /// The property reference identifier.
        /// </value>
        public int PropertyReferenceID { get; set; }

        /// <summary>
        /// Gets or sets The property type identifier
        /// </summary>
        public int PropertyTypeID { get; set; }

        /// <summary>
        /// Gets or sets The offer code
        /// </summary>
        public string OfferCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether The special offers only
        /// </summary>
        public bool SpecialOffersOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether The disabled facilities
        /// </summary>
        public bool DisabledFacilities { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether The adjoining rooms
        /// </summary>
        public bool AdjoiningRooms { get; set; }

        /// <summary>
        /// Gets or sets the meal basis identifier.
        /// </summary>
        /// <value>
        /// The meal basis identifier.
        /// </value>
        public int MealBasisID { get; set; }

        /// <summary>
        /// Gets or sets the star rating.
        /// </summary>
        /// <value>
        /// The star rating.
        /// </value>
        public string StarRating { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the selling country.
        /// </summary>
        /// <value>
        /// The ISO 3166-2 country code.
        /// </value>
        public string SellingCountry { get; set; } = string.Empty;

        /// <summary>
        /// Gets the star rating integer.
        /// </summary>
        /// <value>
        /// The star rating integer.
        /// </value>
        public int StarRatingInteger
        {
            get
            {
                return this.StarRating.ToSafeString().Replace("+", string.Empty).ToSafeInt();
            }
        }

        /// <summary>
        /// Gets or sets The facilities
        /// </summary>
        public ArrayList Facilities { get; set; } = new ArrayList();

        /// <summary>
        /// Gets or sets The features
        /// </summary>
        public ArrayList Features { get; set; } = new ArrayList();

        /// <summary>
        /// Gets or sets the product attribute i ds.
        /// </summary>
        /// <value>
        /// The product attribute i ds.
        /// </value>
        public List<int> ProductAttributeIDs { get; set; } = new List<int>();

        /// <summary>
        /// Gets or sets The passengers
        /// </summary>
        public int Passengers { get; set; } = 0;

        /// <summary>
        /// Gets or sets the room details.
        /// </summary>
        /// <value>
        /// The room details.
        /// </value>
        public RoomDetails RoomDetails { get; set; } = new RoomDetails();

        /// <summary>
        /// Gets or sets a value indicating whether The one way
        /// </summary>
        public bool OneWay { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether The allow multi sector flights
        /// </summary>
        public bool AllowMultisectorFlights { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether The widen search
        /// </summary>
        public bool WidenSearch { get; set; } = true;

        /// <summary>
        /// Gets or sets the selling currency identifier.
        /// </summary>
        /// <value>
        /// The selling currency identifier.
        /// </value>
        public int SellingCurrencyID { get; set; }

        /// <summary>
        /// Gets or sets The priority property identifier
        /// </summary>
        public int PriorityPropertyID { get; set; } = 0;

        /// <summary>
        /// Gets or sets The flight carrier identifier
        /// </summary>
        public int FlightCarrierID { get; set; } = 0;

        /// <summary>
        /// Gets or sets The third party identifier
        /// </summary>
        public int ThirdPartyID { get; set; } = 0;

        /// <summary>
        /// Gets or sets the booking components.
        /// </summary>
        /// <value>
        /// The booking components.
        /// </value>
        public BookingComps BookingComponents { get; set; }

        /// <summary>
        /// Gets or sets The lead guest nationality identifier
        /// </summary>
        public int LeadGuestNationalityID { get; set; } = 0;

        /// <summary>
        /// Gets or sets The lead guest residence geography level1 identifier
        /// </summary>
        public int LeadGuestResidenceGeographyLevel1ID { get; set; } = 0;

        /// <summary>
        /// Gets or sets The lead guest booking country identifier
        /// </summary>
        public int LeadGuestBookingCountryID { get; set; } = 0;

        /// <summary>
        /// Gets or sets The lead guest booking country identifier
        /// </summary>
        public List<int> PropertyReferenceIDs { get; set; } = new List<int>();

        /// <summary>
        /// Gets or sets the resort i ds.
        /// </summary>
        /// <value>
        /// The resort i ds.
        /// </value>
        public List<int> ResortIDs { get; set; } = new List<int>();

        /// <summary>
        /// Gets or sets a value indicating whether The one country
        /// </summary>
        public bool OneCountry { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether The one resort
        /// </summary>
        public bool OneResort { get; set; } = false;

        /// <summary>
        /// Gets or sets The warnings
        /// </summary>
        public ArrayList Warnings { get; set; } = new ArrayList();

        /// <summary>
        /// Gets or sets The referring page
        /// </summary>
        public string ReferrringPage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether The search previous bookings
        /// </summary>
        public bool SearchPreviousBookings { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether The suppress pricing
        /// </summary>
        public bool SuppressPricing { get; set; } = false;

        /// <summary>
        /// Gets or sets The promotional code
        /// </summary>
        public string PromotionalCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The search status
        /// </summary>
        public string SearchStatus { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The XML booking login
        /// </summary>
        public string XMLBookingLogin { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether The select alt flights
        /// </summary>
        public bool SelectAltFlights { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether The deserialize results to class
        /// </summary>
        public bool DeserializeResultsToClass { get; set; } = false;

        /// <summary>
        /// Gets or sets The results
        /// </summary>
        public SearchResults Results
        {
            get
            {
                return new SearchResults()
                {
                    DedupeResults = ConcurrentResults.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                };
            }
            set
            {

            }
        }

        /// <summary>
        /// Gets or sets The results
        /// </summary>
        public ConcurrentDictionary<string, DedupeSearchResult> ConcurrentResults { get; set; } = new();

        /// <summary>
        /// Gets or sets These two are set from the flight supplier setup in the main search routine
        /// </summary>
        public int HlpFlightSearchFromDays { get; set; } = 0;

        /// <summary>
        /// Gets or sets The HLP flight search to days
        /// </summary>
        public int HlpFlightSearchToDays { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether The HLP searched low cost
        /// </summary>
        public bool HlpSearchedLowcost { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether The HLP searched scheduled
        /// </summary>
        public bool HlpSearchedScheduled { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether The HLP search charter
        /// </summary>
        public bool HlpSearchCharter { get; set; } = false;

        /// <summary>
        /// Gets or sets Decides if we're going to de-dupe in code or the database
        /// </summary>
        public VBDedupeMode VBDedupe { get; set; } = VBDedupeMode.Off;

        /// <summary>
        /// Gets or sets a value indicating whether we want to de-dupes.
        /// </summary>
        /// <value>
        ///   A boolean
        /// </value>
        public bool DedupeResults { get; set; } = true;

        /// <summary>
        /// Gets or sets The override de dupe algorithm
        /// </summary>
        public string OverrideDedupeAlgorithm { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the language identifier.
        /// </summary>
        /// <value>
        /// The language identifier.
        /// </value>
        public int LanguageID { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether The ignore property language
        /// </summary>
        public bool IgnorePropertyLanguage { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether The search cached data
        /// </summary>
        public bool SearchCachedData { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether For returning results as xml
        /// </summary>
        public bool ReturnResultsAsXML { get; set; } = false;

        /// <summary>
        /// Gets or sets The results XML
        /// </summary>
        public XmlDocument ResultsXML { get; set; } = new XmlDocument();

        /// <summary>
        /// Gets or sets a value indicating whether is debug
        /// </summary>
        public bool IsDebug { get; set; } = false;

        /// <summary>
        /// Gets or sets The parent search mode
        /// </summary>
        public SearchMode ParentSearchMode { get; set; } = SearchMode.FlightPlusHotel;

        /// <summary>
        /// Gets or sets a value indicating whether [suppress property errata].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [suppress property errata]; otherwise, <c>false</c>.
        /// </value>
        public bool SuppressPropertyErrata { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether [get property errata at property level].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [get property errata at property level]; otherwise, <c>false</c>.
        /// </value>
        public bool GetPropertyErrataAtPropertyLevel { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether [suppress property helpers].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [suppress property helpers]; otherwise, <c>false</c>.
        /// </value>
        public bool SuppressPropertyHelpers { get; set; } = false;

        /// <summary>
        /// Gets the property arrival date.
        /// </summary>
        /// <value>
        /// The property arrival date.
        /// </value>
        public DateTime PropertyArrivalDate => this.ArrivalDate;

        /// <summary>
        /// Gets the duration of the property.
        /// </summary>
        /// <value>
        /// The duration of the property.
        /// </value>
        public int PropertyDuration => this.Duration;

        /// <summary>
        /// Gets or sets The property departure date
        /// </summary>
        public DateTime PropertyDepartureDate => this.DepartureDate;

        /// <summary>
        /// Gets or sets The flight duration
        /// </summary>
        public int FlightDuration { get; set; }

        /// <summary>
        /// Gets or sets the total passengers.
        /// </summary>
        /// <value>
        /// The total passengers.
        /// </value>
        public int TotalPassengers
        {
            get
            {
                int totalPassengers = 0;

                foreach (RoomDetail room in RoomDetails)
                {
                    totalPassengers += room.Adults + room.Children;
                }

                return totalPassengers;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets the total adults.
        /// </summary>
        /// <value>
        /// The total adults.
        /// </value>
        public int TotalAdults
        {
            get
            {
                int totalAdults = 0;

                foreach (RoomDetail room in RoomDetails)
                {
                    totalAdults += room.Adults;
                }

                return totalAdults;
            }
        }

        /// <summary>
        /// Gets the total children.
        /// </summary>
        /// <value>
        /// The total children.
        /// </value>
        public int TotalChildren
        {
            get
            {
                int totalChildren = 0;
                foreach (RoomDetail room in RoomDetails)
                {
                    totalChildren += room.Children;
                }

                return totalChildren;
            }
        }

        /// <summary>
        /// Gets the total infants.
        /// </summary>
        /// <value>
        /// The total infants.
        /// </value>
        public int TotalInfants
        {
            get
            {
                int totalInfants = 0;
                foreach (RoomDetail room in RoomDetails)
                {
                    totalInfants += room.Infants;
                }

                return totalInfants;
            }
        }

        /// <summary>
        /// Gets or sets the base search searching time.
        /// </summary>
        /// <value>
        /// The base search searching time.
        /// </value>
        public decimal BaseSearchSearchingTime { get; set; }

        /// <summary>
        /// Gets or sets the base search start threads time.
        /// </summary>
        /// <value>
        /// The base search start threads time.
        /// </value>
        public decimal BaseSearchStartThreadsTime { get; set; }

        /// <summary>
        /// Gets or sets the base search terminate threads.
        /// </summary>
        /// <value>
        /// The base search terminate threads.
        /// </value>
        public decimal BaseSearchTerminateThreads { get; set; }

        /// <summary>
        /// Gets or sets the HLP tracking affiliate identifier.
        /// </summary>
        /// <value>
        /// The HLP tracking affiliate identifier.
        /// </value>
        public int HlpTrackingAffiliateID { get; set; }

        /// <summary>
        /// Gets or sets the tracking affiliate identifier.
        /// </summary>
        /// <value>
        /// The tracking affiliate identifier.
        /// </value>
        public int TrackingAffiliateID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [store search logging].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [store search logging]; otherwise, <c>false</c>.
        /// </value>
        public bool StoreSearchLogging { get; set; } = false;

        /// <summary>
        /// Get or sets the unique nationality identifier 
        /// </summary>
        /// <value>
        ///  The nationality identifier
        /// </value>
        public string NationalityID { get; set; } = string.Empty;

        /// <summary>
        /// Get or sets the currency code
        /// </summary>
        /// <value>
        ///  The currency code identifier
        /// </value>
        public string CurrencyCode { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the search logs.
        /// </summary>
        /// <value>
        /// The search logs.
        /// </value>
        [XmlIgnore]
        public List<LogCollection> SearchLogs { get; set; } = new List<LogCollection>();

        public int BookingComponentID { get; set; }
        public bool OpaqueSearch { get; set; }
        public bool ExcludeNonRefundable { get; set; }

        public int BrandID { get; set; }
        public int TradeID { get; set; }
        public int SalesChannelID { get; set; }
        public int SellingGeographyLevel1ID { get; set; }

        /// <summary>
        /// Returns all child ages
        /// </summary>
        /// <returns>all child ages</returns>
        public List<int> AllChildAges()
        {
            List<int> childAges = new List<int>();
            foreach (RoomDetail roomDetail in RoomDetails)
            {
                childAges.AddRange(roomDetail.ChildAges);
            }

            return childAges;
        }
    }
}