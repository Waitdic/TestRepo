namespace ThirdParty.Lookups
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

    /// <summary>
    /// An implementation of the third party support, which is used to inject access to settings
    /// </summary>
    /// <seealso cref="ITPSupport" />
    public class TPSupportWrapper : ITPSupport
    {
        private readonly IMemoryCache _cache;
        private readonly ISql _sql;

        public TPSupportWrapper(IMemoryCache cache, ISql sql)
        {
            _cache = Ensure.IsNotNull(cache, nameof(cache));
            _sql = Ensure.IsNotNull(sql, nameof(sql));
        }

       /// <inheritdoc />
        public string CurrencyLookup(int currencyId)
        {
            // todo - implement or remove
            return string.Empty;
        }

        /// <inheritdoc />
        public async Task<int> TPBookingCountryCodeLookupAsync(string source, string bookingCountryCode)
        {
            return (await this.TPBookingCountryCodeAsync(source)).FirstOrDefault(c => c.Key == bookingCountryCode).Value;
        }

        /// <inheritdoc />
        public async Task<string> TPBookingCountryLookupAsync(string source, int bookingCountryId)
        {
            return (await this.TPBookingCountryCodeAsync(source)).FirstOrDefault(c => c.Value == bookingCountryId).Key;
        }

        /// <inheritdoc />
        public async Task<string> TPCountryCodeLookupAsync(string source, string countryCode)
        {
            return (await this.TPCountryCodeAsync(source)).FirstOrDefault(c => c.Key == countryCode).Value;
        }

        /// <inheritdoc />
        public async Task<string> TPCurrencyLookupAsync(string source, string currencyCode)
        {
            return (await this.TPCurrencyAsync(source)).FirstOrDefault(c => c.Key == currencyCode).Value;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, int>> TPMealBasesAsync(string source)
        {
            string cacheKey = "TPMealBasisLookup_" + source;

            async Task<Dictionary<string, int>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select MealBasisCode, MealBasisID from Mealbasis where Source = @source",
                    async r => (await r.ReadAllAsync<MealBasis>()).ToDictionary(x => x.MealBasisCode, x => x.MealBasisID),
                    new CommandSettings().WithParameters(new { source }));
            }

            return await _cache.GetOrCreateAsync(cacheKey, cacheBuilder, 60);
        }

        /// <inheritdoc />
        public async Task<string> TPMealBasisLookupAsync(string source, int mealBasisId)
        {
            return (await this.TPMealBasesAsync(source)).FirstOrDefault(c => c.Value == mealBasisId).Key;
        }

        /// <inheritdoc />
        public async Task<int> TPMealBasisLookupAsync(string source, string mealBasis)
        {
            (await this.TPMealBasesAsync(source)).TryGetValue(mealBasis, out int mealBasisId);

            return mealBasisId;
        }

        /// <inheritdoc />
        public async Task<string> TPNationalityLookupAsync(string source, string nationality)
        {
            nationality = Regex.Replace(nationality, @"\s+", string.Empty); // remove all line breaks and whitespaces 
            return (await this.TPNationalityAsync(source)).FirstOrDefault(c => c.Key == nationality).Value;
        }

        /// <summary>Third party currency lookup.</summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        private async Task<Dictionary<string, string>> TPCurrencyAsync(string source)
        {
            string cacheKey = "TPCurrencyLookup_" + source;

            async Task<Dictionary<string, string>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select ThirdPartyCurrencyCode, CurrencyCode from Currency where Source = @source",
                    async r => (await r.ReadAllAsync<Currency>()).ToDictionary(x => x.CurrencyCode, x => x.ThirdPartyCurrencyCode),
                    new CommandSettings().WithParameters(new { source }));
            }

            return await _cache.GetOrCreateAsync(cacheKey, cacheBuilder, 60);
        }

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

        /// <summary>Booking country lookup</summary>
        /// <param name="source">The source.</param>
        /// <returns>a dictionary of third party booking country and booking country identifier</returns>
        private async Task<Dictionary<string, int>> TPBookingCountryCodeAsync(string source)
        {
            string cacheKey = "TPBookingCountryCodeLookup_" + source;

            async Task<Dictionary<string, int>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select TPBookingCountryCode, BookingCountryLookupID from BookingCountryLookup where Source = @source",
                    async r => (await r.ReadAllAsync<BookingCountry>()).ToDictionary(x => x.TPBookingCountryCode, x => x.BookingCountryLookupID),
                    new CommandSettings().WithParameters(new { source }));
            }

            return await _cache.GetOrCreateAsync(cacheKey, cacheBuilder, 60);
        }

        private class BookingCountry
        {
            public string TPBookingCountryCode { get; set; } = string.Empty;
            public int BookingCountryLookupID { get; set; }
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

            return await _cache.GetOrCreateAsync(cacheKey, cacheBuilder, 60);
        }

        private class Nationality
        {
            public string NationalityCode { get; set; } = string.Empty;
            public string ISOCode { get; set; } = string.Empty;
        }

        /// <inheritdoc />
        public async Task<int> TPCurrencyIDLookupAsync(string currencyCode)
        {
            // todo - caching
            return await _sql.ReadScalarAsync<int>(
                "select ISOCurrencyID from ISOCurrency where CurrencyCode = @currencyCode",
                new CommandSettings().WithParameters(new { currencyCode }));
        }

        /// <inheritdoc />
        public async Task<string> TPCurrencyCodeLookupAsync(int currencyId)
        {
            // todo - caching
            return await _sql.ReadScalarAsync<string>(
                "select CurrencyCode from ISOCurrency where ISOCurrencyID = @currencyID",
                new CommandSettings().WithParameters(new { currencyId }));
        }

        /// <summary>Country code lookup</summary>
        /// <param name="source">The source.</param>
        /// <returns>a dictionary of ISO country code and thirdparty country code</returns>
        private async Task<Dictionary<string, string>> TPCountryCodeAsync(string source)
        {
            string cacheKey = "TPCountyCodeLookup_" + source;

            async Task<Dictionary<string, string>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select ISOCode, TPCountryCode from CountryCodeLookup where Source = @source",
                    async r => (await r.ReadAllAsync<Country>()).ToDictionary(x => x.ISOCode, x => x.TPCountryCode),
                    new CommandSettings().WithParameters(new { source }));
            }

            return await _cache.GetOrCreateAsync(cacheKey, cacheBuilder, 60);
        }

        private class Country
        {
            public string ISOCode { get; set; } = string.Empty;
            public string TPCountryCode { get; set; } = string.Empty;
        }

        /// <inheritdoc />
        public async Task<string> TPResortCodeByPropertyIdLookupAsync(string source, int propertyId)
        {
            return (await TPResortCodeByPropertyIdLookupAsync(source))
                .TryGetValue(propertyId, out string resortCode) ? resortCode : string.Empty;
        }

        private async Task<Dictionary<int, string>> TPResortCodeByPropertyIdLookupAsync(string source)
        {
            string cacheKey = "TPResortCodeByPropertyIdLookup_" + source;

            async Task<Dictionary<int, string>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    @"select distinct Property.PropertyID, Geography.Code from Property
                        inner join Geography on Property.GeographyID = Geography.GeographyID
                        where Property.Source = @source",
                    async r => (await r.ReadAllAsync<Property>()).ToDictionary(x => x.PropertyID, x => x.Code),
                    new CommandSettings().WithParameters(new { source }));
            }

            return await _cache.GetOrCreateAsync(cacheKey, cacheBuilder, 60);
        }

        private class Property
        {
            public int PropertyID { get; set; }
            public string Code { get; set; } = string.Empty;
        }

        /// <inheritdoc />
        public async Task<string> TPResortCodeByGeographyIdLookupAsync(string source, int geographyId)
        {
            return (await TPResortCodeByGeographyIdLookupAsync(source))
                .TryGetValue(geographyId, out string resortCode) ? resortCode : string.Empty;
        }

        private async Task<Dictionary<int, string>> TPResortCodeByGeographyIdLookupAsync(string source)
        {
            string cacheKey = "TPResortCodeByGeographyIdLookup_" + source;

            async Task<Dictionary<int, string>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select GeographyID, Code from Geography where Source = @source",
                    async r => (await r.ReadAllAsync<Geography>()).ToDictionary(x => x.GeographyID, x => x.Code),
                    new CommandSettings().WithParameters(new { source }));
            }

            return await _cache.GetOrCreateAsync(cacheKey, cacheBuilder, 60);
        }

        private class Geography
        {
            public int GeographyID { get; set; }
            public string Code { get; set; } = string.Empty;
        }

        public string TPCreditCardLookup(string source, int creditCardTypeId)
        {
            // todo - implement or remove
            throw new NotImplementedException();
        }
    }
}