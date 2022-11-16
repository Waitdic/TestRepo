namespace iVectorOne.Factories
{
    using iVectorOne.Factories;
    using iVectorOne.Models;
    using iVectorOne.SDK.V2.TransferSearch;
    using iVectorOne.Search.Models;
    using System;

    /// <summary>
    /// A factory that creates a search details factory using a search request
    /// </summary>
    /// <seealso cref="ITransferSearchDetailsFactory" />
    public class TransferSearchDetailsFactory : ITransferSearchDetailsFactory
    {
        /// <summary>Creates the specified search request.</summary>
        /// <param name="searchRequest">The search request.</param>
        /// <param name="account">The account, used for retrieving configuration and settings</param>
        /// /// <param name="log">boolean that decides if we log third party requests and responses</param>
        /// <returns>A search details</returns>
        public TransferSearchDetails Create(Request searchRequest, Account account, bool log)
        {
            var searchDetails = new TransferSearchDetails()
            {
                DepartureLocationId = searchRequest.DepartureLocationID,
                ArrivalLocationId = searchRequest.ArrivalLocationID,

                OneWay = !searchRequest.ReturnDate.HasValue,

                Adults = searchRequest.Adults,
                Children = searchRequest.Children,
                Infants = searchRequest.Infants,
                ChildAges = searchRequest.ChildAges,

                Source = searchRequest.Supplier,

                AccountID = account.AccountID,
                Settings = account.TPSettings,
                ThirdPartyConfigurations = account.Configurations,
                LoggingType = log ? "All" : "None",
                ISOCurrencyCode = string.IsNullOrEmpty(searchRequest.CurrencyCode) ? account.TPSettings.CurrencyCode : searchRequest.CurrencyCode,
                EmailLogsToAddress = searchRequest.EmailLogsToAddress,

                //SearchStoreItem =
                //{
                //    AccountName = account.Login,
                //    AccountId = account.AccountID,
                //    System = account.Environment.ToString(),
                //    SearchDateAndTime = DateTime.Now,
                //    PropertiesRequested = searchRequest.Properties.Count
                //}

            };

            if (searchRequest.DepartureDate.HasValue)
            {
                searchDetails.DepartureDate = searchRequest.DepartureDate.Value;
                searchDetails.DepartureTime = searchRequest.DepartureTime;
            }

            if (searchRequest.ReturnDate.HasValue)
            {
                searchDetails.ReturnDate = searchRequest.ReturnDate.Value;
                searchDetails.ReturnTime = searchRequest.ReturnTime;
            }

            return searchDetails;
        }
    }
}
