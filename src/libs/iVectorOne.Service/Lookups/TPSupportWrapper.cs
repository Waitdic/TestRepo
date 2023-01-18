namespace iVectorOne.Lookups
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using Intuitive.Helpers.Extensions;
    using Microsoft.Extensions.Caching.Memory;
    using iVectorOne.Constants;
    using iVectorOne.Search.Models;
    using iVectorOne.Models;
    using iVectorOne.Models.Transfer;
    using iVectorOne.Models.Extra;

    /// <summary>
    /// An implementation of the third party support, which is used to inject access to settings
    /// </summary>
    /// <seealso cref="ITPSupport" />
    public class TPSupportWrapper : ITPSupport
    {
        private const int _timeout = 2;

        private readonly IMemoryCache _cache;
        private readonly ISql _sql;

        public TPSupportWrapper(IMemoryCache cache, ISql sql)
        {
            _cache = Ensure.IsNotNull(cache, nameof(cache));
            _sql = Ensure.IsNotNull(sql, nameof(sql));
        }

        #region Lookups

        /// <inheritdoc />
        public async Task<string> TPCountryCodeLookupAsync(string source, string isoCode, int accountId)
        {
            string countryCode;
            if (IsSingleTenant(source))
            {
                (await this.AccountCountryAsync()).TryGetValue((accountId, isoCode), out var country);
                countryCode = country.CountryCode;
            }
            else
            {
                (await this.TPCountryCodeAsync(source)).TryGetValue(isoCode, out var country);
                countryCode = country.TPCountryCode;
            }
            return countryCode;
        }

        public async Task<LocationMapping> TPLocationLookupAsync(TransferSearchDetails searchDetails)
        {
            LocationMapping locationData;

            Dictionary<int, string> location = await this.TPLocationAsync(searchDetails.Source);
            location.TryGetValue(searchDetails.DepartureLocationId, out var departurePayload);
            location.TryGetValue(searchDetails.ArrivalLocationId, out var arivalPayload);
            var AdditionalDeparturePayload = location.Where(x => searchDetails.AdditionalDepartureLocationIDs.Contains(x.Key)).Select(x => x.Value).ToList();
            var AdditionalArrivalPayload = location.Where(x => searchDetails.AdditionalArrivalLocationIDs.Contains(x.Key)).Select(x => x.Value).ToList();

            locationData = new()
            {
                DepartureData = departurePayload,
                ArrivalData = arivalPayload,
                AdditionalDepartureData = AdditionalDeparturePayload,
                AdditionalArrivalData = AdditionalArrivalPayload,
            };

            return locationData;
        }

        public async Task<List<Extras>> TPExtraLookupAsync(ExtraSearchDetails searchDetails)
        {
            var extras = await this.TPExtrasAsync(searchDetails.Source);
            List<Extras> extrasFilteredList = extras.Where(x => searchDetails.ExtraIDs.Contains(x.ExtraID)).ToList();

            return extrasFilteredList;
        }

        public async Task<List<string>> TPAllLocationLookup(string source)
        {
            Dictionary<int, string> location = await this.TPLocationAsync(source);
            return location.Select(x => x.Value).ToList();
        }

        /// <inheritdoc />
        public async Task<string> TPCountryLookupAsync(string source, string isoCode, int accountId)
        {
            string countryName;
            if (IsSingleTenant(source))
            {
                (await this.AccountCountryAsync()).TryGetValue((accountId, isoCode), out var country);
                countryName = country.Country;
            }
            else
            {
                (await this.TPCountryCodeAsync(source)).TryGetValue(isoCode, out var country);
                countryName = country.Country;
            }
            return countryName;
        }

        /// <inheritdoc />
        public async Task<string> TPCurrencyLookupAsync(string source, string currencyCode)
        {
            (await this.TPCurrencyAsync(source)).TryGetValue(currencyCode, out string isoCode);
            return isoCode;
        }

        /// <inheritdoc />
        public async Task<string> TPCurrencyCodeLookupAsync(string source, string isoCode)
        {
            return (await this.TPCurrencyAsync(source)).FirstOrDefault(c => c.Value == isoCode).Key;
        }

        /// <inheritdoc />
        public async Task<string> TPNationalityLookupAsync(string source, string nationality)
        {
            nationality = Regex.Replace(nationality, @"\s+", string.Empty); // remove all line breaks and whitespaces 
            (await this.TPNationalityAsync(source)).TryGetValue(nationality, out string isoCode);
            return isoCode;
        }

        /// <inheritdoc />
        public async Task<int> ISOCurrencyIDLookupAsync(string currencyCode)
        {
            return (await ISOCurrencyAsync()).FirstOrDefault(x => x.Value == currencyCode).Key;
        }

        /// <inheritdoc />
        public async Task<string> ISOCurrencyCodeLookupAsync(int isoCurrencyId)
        {
            (await ISOCurrencyAsync()).TryGetValue(isoCurrencyId, out string currencyCode);
            return currencyCode;
        }

        public string TPCreditCardLookup(string source, int creditCardTypeId)
        {
            // todo - implement or remove
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<string> SupplierNameLookupAsync(int supplierId)
        {
            (await SupplierAsync()).TryGetValue(supplierId, out string source);
            return source;
        }

        public async Task<int> SupplierIDLookupAsync(string supplierName)
        {
            return (await SupplierAsync()).FirstOrDefault(x => x.Value == supplierName).Key;
        }

        public void RemoveLocationCache(string source)
        {
            string cacheKey = "TPLocationLookup_" + source;
            _cache.Remove(cacheKey);
        }

        private bool IsSingleTenant(string source)
            => source.InList(ThirdParties.OWNSTOCK, ThirdParties.CHANNELMANAGER);

        #endregion

        #region Cache Builders

        /// <inheritdoc />
        public async Task<Dictionary<string, int>> TPMealBasesAsync(string source)
        {
            string cacheKey = "TPMealBasisLookup_" + source;

            async Task<Dictionary<string, int>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select MealBasisCode, MealBasisID from MealBasis where Source = @source",
                    async r => (await r.ReadAllAsync<MealBasis>()).ToDictionary(x => x.MealBasisCode, x => x.MealBasisID),
                    new CommandSettings().WithParameters(new { source }));
            }

            return await _cache.GetOrCreateAsync(cacheKey, cacheBuilder, _timeout);
        }

        /// <summary>Third party currency lookup.</summary>
        /// <param name="source">The source.</param>
        /// <returns>The currency cache</returns>
        private async Task<Dictionary<string, string>> TPCurrencyAsync(string source)
        {
            string cacheKey = "TPCurrencyLookup_" + source;

            async Task<Dictionary<string, string>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select ThirdPartyCurrencyCode, CurrencyCode from Currency where Source = @source",
                    async r => (await r.ReadAllAsync<Currency>()).ToDictionary(x => x.ThirdPartyCurrencyCode, x => x.CurrencyCode),
                    new CommandSettings().WithParameters(new { source }));
            }

            return await _cache.GetOrCreateAsync(cacheKey, cacheBuilder, _timeout);
        }

        /// <summary>Nationality lookup</summary>
        /// <param name="source">The source.</param>
        /// <returns>a dictionary of third party national identifier and corrsponding ISO code</returns>
        private async Task<Dictionary<string, string>> TPNationalityAsync(string source)
        {
            string cacheKey = "TPNationalityLookup_" + source;

            async Task<Dictionary<string, string>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select NationalityCode, ISOCode from NationalityLookup where Source = @source",
                    async r => (await r.ReadAllAsync<Nationality>()).ToDictionary(x => x.NationalityCode, x => x.ISOCode),
                    new CommandSettings().WithParameters(new { source }));
            }

            return await _cache.GetOrCreateAsync(cacheKey, cacheBuilder, _timeout);
        }

        /// <summary>ISO currency code lookup</summary>
        /// <returns>A dictionary of ISO currency codes</returns>
        private async Task<Dictionary<int, string>> ISOCurrencyAsync()
        {
            async Task<Dictionary<int, string>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select ISOCurrencyID, CurrencyCode from ISOCurrency",
                    async r => (await r.ReadAllAsync<ISOCurrency>()).ToDictionary(x => x.ISOCurrencyID, x => x.CurrencyCode),
                    new CommandSettings());
            }

            return await _cache.GetOrCreateAsync("ISOCurrencyCache", cacheBuilder, _timeout);
        }

        /// <summary>Country code lookup</summary>
        /// <param name="source">The source.</param>
        /// <returns>a dictionary of ISO country code and thirdparty country code</returns>
        private async Task<Dictionary<string, CountryLookup>> TPCountryCodeAsync(string source)
        {
            string cacheKey = "TPCountyCodeLookup_" + source;

            async Task<Dictionary<string, CountryLookup>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select ISOCode, TPCountryCode, Country from CountryCodeLookup where Source = @source",
                    async r => (await r.ReadAllAsync<CountryLookup>()).ToDictionary(x => x.ISOCode, x => x),
                    new CommandSettings().WithParameters(new { source }));
            }

            return await _cache.GetOrCreateAsync(cacheKey, cacheBuilder, _timeout);
        }

        private async Task<Dictionary<int, string>> TPLocationAsync(string source)
        {
            string cacheKey = "TPLocationLookup_" + source;

            async Task<Dictionary<int, string>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select IVOLocationID, Payload  from IVOLocation where Source = @source",
                    async r => (await r.ReadAllAsync<LocationLookup>()).ToDictionary(x => x.IVOLocationID, x => x.Payload),
                    new CommandSettings().WithParameters(new { source }));
            }

            return await _cache.GetOrCreateAsync(cacheKey, cacheBuilder, _timeout);
        }

        private async Task<Dictionary<(int, string), AccountCountry>> AccountCountryAsync()
        {
            string cacheKey = "AccountCountryLookup";

            async Task<Dictionary<(int, string), AccountCountry>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select AccountID, CountryCode, ISOCountryCode, Country from AccountCountry",
                    async r => (await r.ReadAllAsync<AccountCountry>())
                        .ToDictionary(x => (x.AccountID, x.ISOCountryCode), x => x));
            }

            var cache = await _cache.GetOrCreateAsync(cacheKey, cacheBuilder, _timeout);

            return cache;
        }

        /// <summary>Supplier name lookup</summary>
        /// <returns>A dictionary of suppliers</returns>
        private async Task<Dictionary<int, string>> SupplierAsync()
        {
            async Task<Dictionary<int, string>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select SupplierID, SupplierName from Supplier",
                    async r => (await r.ReadAllAsync<Supplier>()).ToDictionary(x => x.SupplierID, x => x.SupplierName),
                    new CommandSettings());
            }

            return await _cache.GetOrCreateAsync("SupplierCache", cacheBuilder, _timeout);
        }

        /// <summary>Extras lookup</summary>
        /// <returns>A Collection of Extras</returns>
        private async Task<List<Extras>> TPExtrasAsync(string source)
        {
            string cacheKey = "TPExtrasLookup_" + source;

            async Task<List<Extras>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select ExtraID, ExtraName,Payload  from Extra where Source = @source",
                    async r => (await r.ReadAllAsync<Extras>()).ToList(),
                    new CommandSettings().WithParameters(new { source }));
            }

            return await _cache.GetOrCreateAsync(cacheKey, cacheBuilder, _timeout);
        }


        public async Task<TransferBookingDetails> GetTransferBookingDetailsAsync(string supplierBookingReference)
        {
            return await _sql.ReadSingleAsync<TransferBookingDetails>(
                        "TransferBooking_GetData",
                        new CommandSettings()
                        .IsStoredProcedure()
                        .WithParameters(new
                        {
                            supplierBookingReference = supplierBookingReference
                        }));
        }

        #endregion

        #region DTO classes

        private class Currency
        {
            public string ThirdPartyCurrencyCode { get; set; } = string.Empty;
            public string CurrencyCode { get; set; } = string.Empty;
        }

        private class MealBasis
        {
            public string MealBasisCode { get; set; } = string.Empty;
            public int MealBasisID { get; set; }
        }

        private class BookingCountry
        {
            public string TPBookingCountryCode { get; set; } = string.Empty;
            public string BookingCountryCode { get; set; } = string.Empty;
        }

        private class Nationality
        {
            public string NationalityCode { get; set; } = string.Empty;
            public string ISOCode { get; set; } = string.Empty;
        }

        private class ISOCurrency
        {
            public int ISOCurrencyID { get; set; }
            public string CurrencyCode { get; set; } = string.Empty;
        }

        private class CountryLookup
        {
            public string ISOCode { get; set; } = string.Empty;
            public string TPCountryCode { get; set; } = string.Empty;
            public string Country { get; set; } = string.Empty;
        }

        private class LocationLookup
        {
            public int IVOLocationID { get; set; }
            public string Payload { get; set; } = string.Empty;
        }

        private class AccountCountry
        {
            public int AccountID { get; set; }
            public string CountryCode { get; set; } = string.Empty;
            public string ISOCountryCode { get; set; } = string.Empty;
            public string Country { get; set; } = string.Empty;
        }

        private class Supplier
        {
            public int SupplierID { get; set; }
            public string SupplierName { get; set; } = string.Empty;
        }
        #endregion
    }
}