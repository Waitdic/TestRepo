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
    using iVectorOne.SDK.V2.TransferSearch;
    using iVectorOne.Search.Results.Models;

    /// <summary>
    /// A transfer search details object
    /// </summary>
    /// <seealso cref="IThirdPartyAttributeSearch" />
    public class TransferSearchDetails : IThirdPartyAttributeSearch 
    {
        /// <summary>
        /// Gets or sets the account identifier
        /// </summary>
        public int AccountID { get; set; }

        /// <summary>
        /// Gets or sets the departure location identifier
        /// </summary>
        public int DepartureLocationId { get; set; }

        // <summary>
        /// Gets or sets the arrival location identifier
        /// </summary>
        public int ArrivalLocationId { get; set; }

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
        public TransferSearchResults Results { get; set; } = new TransferSearchResults();
        ///public List<TransformedTransferResult> Results { get; set; } = new List<TransformedTransferResult>();

        public string Source { get; set; } = string.Empty;

        List<ThirdPartyConfiguration> IThirdPartyAttributeSearch.ThirdPartyConfigurations { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <value>
        /// The duration. Zero for one way
        /// </value>
        public int Duration => OneWay ? 0 : (ReturnDate - DepartureDate).Days;


        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>
        /// The session identifier.
        /// </value>
        //public string SessionID { get; set; } = string.Empty;




        /// <summary>
        /// Gets or sets The departure secondary time
        /// </summary>
        //public string DepartureSecondaryTime { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The departure flight code
        /// </summary>
        //public string DepartureFlightCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The return secondary time
        /// </summary>
        //public string ReturnSecondaryTime { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets The return flight code
        /// </summary>
        //public string ReturnFlightCode { get; set; } = string.Empty;



        /// <summary>
        /// Gets or sets the number of youths.
        /// </summary>
        /// <value>
        /// The number of youths.
        /// </value>
        ///public int Youths { get; set; }

        /// <summary>
        /// Gets or sets the number of seniors.
        /// </summary>
        /// <value>
        /// The number of seniors.
        /// </value>
        //public int Seniors { get; set; }

        /// <summary>
        /// Gets or sets The supplier identifier
        /// </summary>
        //public int SupplierID { get; set; } = 0;

        /// <summary>
        /// Gets or sets The vehicle type
        /// </summary>
        //public string VehicleType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the booking date.
        /// </summary>
        /// <value>
        /// The booking date.
        /// </value>
        public DateTime BookingDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the IATA code
        /// </summary>
        //public string IATACode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the arrival resort identifier
        /// </summary>
        //public int ArrivalResortID { get; set; } = 0;


        /// <summary>
        /// Gets or sets the senior ages
        /// </summary>
        //public List<int> SeniorAges { get; set; } = new List<int>();


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
        public ArrayList Warnings { get; set; } = new ArrayList();

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




    }
}