namespace ThirdParty.Factories
{
    using iVector.Search.Property;
    using ThirdParty.Models;
    using ThirdParty.SDK.V2.PropertySearch;
    using ThirdParty.Search.Models;

    /// <summary>
    /// A factory that creates a search details factory using a search request
    /// </summary>
    /// <seealso cref="ISearchDetailsFactory" />
    public class SearchDetailsFactory : ISearchDetailsFactory
    {
        /// <summary>Creates the specified search request.</summary>
        /// <param name="searchRequest">The search request.</param>
        /// <param name="user">The user, used for retrieving configuration and settings</param>
        /// /// <param name="log">boolean that decides if we log third party requests and responses</param>
        /// <returns>A search details</returns>
        public SearchDetails Create(Request searchRequest, User user, bool log)
        {
            var searchDetails = new SearchDetails()
            {
                Duration = searchRequest.Duration,
                PropertyReferenceIDs = searchRequest.Properties,
                Rooms = searchRequest.RoomRequests.Count,
                NationalityID = searchRequest.NaionalityID,
                Settings = user.TPSettings,
                ThirdPartyConfigurations = user.Configurations,
                LoggingType = log ? "All" : "None",
                CurrencyCode = string.IsNullOrEmpty(searchRequest.CurrencyCode) ? user.TPSettings.CurrencyCode : searchRequest.CurrencyCode,
                OpaqueSearch = searchRequest.OpaqueRates,
                SellingCountry = searchRequest.SellingCountry,
            };

            if (searchRequest.ArrivalDate.HasValue)
            {
                searchDetails.ArrivalDate = searchRequest.ArrivalDate.Value;
                searchDetails.DepartureDate = searchRequest.ArrivalDate.Value.AddDays(searchRequest.Duration);
            }

            foreach (var roomRequest in searchRequest.RoomRequests)
            {
                var roomDetails = new RoomDetail(
                    roomRequest.Adults,
                    roomRequest.Children,
                    roomRequest.Infants,
                    string.Join(",", roomRequest.ChildAges));
                searchDetails.RoomDetails.Add(roomDetails);
            }

            return searchDetails;
        }
    }
}