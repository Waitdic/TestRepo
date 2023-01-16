namespace iVectorOne.Search.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;
    using iVectorOne.Models;
    using iVectorOne.Models.SearchStore;
    using iVectorOne.SDK.V2.ExtraSearch;
    using iVectorOne.Search.Results.Models;

    /// <summary>
    /// A extra search details object
    /// </summary>
    /// <seealso cref="IThirdPartyAttributeSearch" />
    public class ExtraSearchDetails : IThirdPartyAttributeSearch 
    {
        /// <summary>
        /// Gets or sets the account identifier
        /// </summary>
        public int AccountID { get; set; }

        /// <summary>
        /// Gets or sets the list of extra IDs
        /// </summary>
        public List<int> ExtraIDs { get; set; }

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
        /// Gets or sets a value indicating whether the extra is one way
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
        /// Gets or sets the number of adults.
        /// </summary>
        /// <value>
        /// The number of adults.
        /// </value>
        public int Adults { get; set; }

        /// <summary>
        /// Gets or sets the number of children.
        /// </summary>
        /// <value>
        /// The number of children.
        /// </value>
        public int Children { get; set; }

        /// <summary>
        /// Gets or sets the number of infants.
        /// </summary>
        /// <value>
        /// The number of infants.
        /// </value>
        public int Infants { get; set; }

        /// <summary>
        /// Gets or sets the child ages
        /// </summary>
        public List<int> ChildAges { get; set; } = new List<int>();

        /// <summary>
        /// Get or sets the ISO currency code
        /// </summary>
        public string ISOCurrencyCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The results
        /// </summary>
        public ExtraSearchResults Results { get; set; } = new ExtraSearchResults();
        ///public List<TransformedTransferResult> Results { get; set; } = new List<TransformedTransferResult>();

        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <value>
        /// The duration. Zero for one way
        /// </value>
        public int Duration => OneWay ? 0 : (ReturnDate - DepartureDate).Days;

        /// <summary>
        /// Gets or sets the booking date.
        /// </summary>
        /// <value>
        /// The booking date.
        /// </value>
        public DateTime BookingDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets a value indicating whether is debug
        /// </summary>
        public bool IsDebug { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether [store search logging].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [store search logging]; otherwise, <c>false</c>.
        /// </value>
        public bool StoreSearchLogging { get; set; } = false;

        /// <summary>
        /// Gets or sets The warnings
        /// </summary>
        public Warnings Warnings { get; set; } = new ();

        /// <summary>Gets or sets the settings.</summary>
        public Settings Settings { get; set; } = new();

        /// <summary>Gets or sets the third party configurations.</summary>
        public List<ThirdPartyConfiguration> ThirdPartyConfigurations { get; set; } = new();

        /// <summary>Gets or sets the type of the logging.</summary>
        public string LoggingType { get; set; } = "None";

        /// <summary>
        /// Gets or sets The email logs to address
        /// </summary>
        public string EmailLogsToAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the extra search store item
        /// </summary>
        public ExtraSearchStoreItem SearchStoreItem { get; set; } = new() { ExtraSearchStoreId = Guid.NewGuid() };

        /// <summary>
        /// Gets or sets the third party settings
        /// </summary>
        public Dictionary<string, string> ThirdPartySettings { get; set; } = new Dictionary<string, string> { };
    }
}