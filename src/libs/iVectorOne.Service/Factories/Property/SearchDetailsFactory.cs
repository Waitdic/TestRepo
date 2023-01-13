using System;

namespace iVectorOne.Factories
{
    using iVector.Search.Property;
    using iVectorOne.Models;
    using iVectorOne.SDK.V2.PropertySearch;
    using iVectorOne.Search.Models;

    /// <summary>
    /// A factory that creates a search details factory using a search request
    /// </summary>
    /// <seealso cref="ISearchDetailsFactory" />
    public class SearchDetailsFactory : ISearchDetailsFactory
    {
        /// <summary>Creates the specified search request.</summary>
        /// <param name="searchRequest">The search request.</param>
        /// <param name="account">The account, used for retrieving configuration and settings</param>
        /// /// <param name="log">boolean that decides if we log third party requests and responses</param>
        /// <returns>A search details</returns>
        public SearchDetails Create(Request searchRequest, Account account, bool log)
        {
            var searchDetails = new SearchDetails()
            {
                Duration = searchRequest.Duration,
                ISONationalityCode = searchRequest.NationalityID,
                Account = account,
                Settings = account.TPSettings,
                ThirdPartyConfigurations = account.Configurations,
                LoggingType = log ? "All" : "None",
                ISOCurrencyCode = string.IsNullOrEmpty(searchRequest.CurrencyCode) ? account.TPSettings.CurrencyCode : searchRequest.CurrencyCode,
                OpaqueSearch = searchRequest.OpaqueRates,
                SellingCountry = searchRequest.SellingCountry,
                EmailLogsToAddress = searchRequest.EmailLogsToAddress,
                DedupeResults = searchRequest.DedupeMethod,
                SearchStoreItem =
                {
                    AccountName = account.Login,
                    AccountId = account.AccountID,
                    System = account.Environment.ToString(),
                    SearchDateAndTime = DateTime.Now,
                    PropertiesRequested = searchRequest.Properties.Count
                }
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