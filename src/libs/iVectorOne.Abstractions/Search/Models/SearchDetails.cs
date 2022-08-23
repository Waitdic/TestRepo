namespace iVectorOne.Search.Models
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive.Helpers.Extensions;
    using iVector.Search.Property;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Search.Enums;

    /// <summary>
    /// A search details object
    /// </summary>
    /// <seealso cref="IThirdPartyAttributeSearch" />
    public class SearchDetails : IThirdPartyAttributeSearch
    {
        /// <summary>
        /// Gets or sets the account identifier
        /// </summary>
        public int AccountID { get; set; }

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

        // todo - remove or set in factory
        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public decimal Latitude { get; set; } = 0;

        // todo - remove or set in factory
        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public decimal Longitude { get; set; } = 0;

        // todo - remove or set in factory
        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        public decimal Radius { get; set; } = 0;

        /// <summary>
        /// Gets or sets the booking date.
        /// </summary>
        public DateTime BookingDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the arrival date.
        /// </summary>
        public DateTime ArrivalDate { get; set; } = DateTimeExtensions.EmptyDate;

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the departure date.
        /// </summary>
        public DateTime DepartureDate { get; set; }

        /// <summary>
        /// Gets the number of rooms
        /// </summary>
        public int Rooms => RoomDetails.Count();

        // todo - remove or set in factory
        /// <summary>
        /// Gets or sets the star rating.
        /// </summary>
        public string StarRating { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the selling country.
        /// </summary>
        public string SellingCountry { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the room details.
        /// </summary>
        public RoomDetails RoomDetails { get; set; } = new RoomDetails();

        /// <summary>
        /// Gets or sets a value indicating whether we want to de-dupes.
        /// </summary>
        public Dedupe DedupeResults { get; set; } = Dedupe.cheapestleadin;

        /// <summary>
        /// Gets or sets the total passengers.
        /// </summary>
        public int TotalPassengers => RoomDetails.Sum(r => r.Adults + r.Children);

        /// <summary>
        /// Get or sets the unique nationality code 
        /// </summary>
        public string ISONationalityCode { get; set; } = string.Empty;

        /// <summary>
        /// Get or sets the ISO currency code
        /// </summary>
        public string ISOCurrencyCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether we search for opaque rates
        /// </summary>
        public bool OpaqueSearch { get; set; }

        /// <summary>
        /// Gets the total adults.
        /// </summary>
        public int TotalAdults => RoomDetails.Sum(r => r.Adults);

        /// <summary>
        /// Gets the total children.
        /// </summary>
        public int TotalChildren => RoomDetails.Sum(r => r.Children);

        /// <summary>
        /// Gets the total infants.
        /// </summary>
        public int TotalInfants => RoomDetails.Sum(r => r.Infants);

        /// <summary>
        /// Returns all child ages
        /// </summary>
        public List<int> AllChildAges => RoomDetails.SelectMany(r => r.ChildAges).ToList();

        /// <summary>
        /// Gets or sets the results
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
        /// Gets or sets the results concurrently
        /// </summary>
        public ConcurrentDictionary<string, DedupeSearchResult> ConcurrentResults { get; set; } = new();

        /// <summary>
        /// Gets or sets the paging token collector
        /// </summary>
        public IPagingTokenCollector? PagingTokenCollector { get; set; }
    }
}