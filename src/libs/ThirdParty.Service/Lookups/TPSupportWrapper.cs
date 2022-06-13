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

        /// <summary>
        /// Currencies lookup.
        /// </summary>
        /// <param name="currencyID">The currency identifier.</param>
        /// <returns>
        /// The currency that matches the provided currencyID
        /// </returns>
        /// <exception cref="NotImplementedException">not implemented yet</exception>
        public string CurrencyLookup(int currencyID)
        {
            return string.Empty;
        }

        /// <summary>
        /// The booking country code lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="bookingCountryCode">The booking country code.</param>
        /// <returns>
        /// booking country code
        /// </returns>
        /// <exception cref="NotImplementedException">not implemented yet</exception>
        public int TPBookingCountryCodeLookup(string source, string bookingCountryCode)
        {
            return this.TPBookingCountryCode(source).FirstOrDefault(c => c.Key == bookingCountryCode).Value;
        }

        /// <summary>
        /// Returns the booking country.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="bookingCountryID">The booking country identifier.</param>
        /// <returns>
        /// Booking country
        /// </returns>
        /// <exception cref="NotImplementedException">not implemented yet</exception>
        public string TPBookingCountryLookup(string source, int bookingCountryID)
        {
            return this.TPBookingCountryCode(source).FirstOrDefault(c => c.Value == bookingCountryID).Key;
        }

        /// <summary>
        /// The country code lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="countryCode">The iso country code.</param>
        /// <returns>
        /// The third party country code or null if not found 
        /// </returns>
        public string TPCountryCodeLookup(string source, string countryCode)
        {
            return this.TPCountryCode(source).FirstOrDefault(c => c.Key == countryCode).Value;
        }

        /// <summary>
        /// The credit card lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="creditCardTypeId">The credit card type identifier.</param>
        /// <returns>
        /// credit card
        /// </returns>
        /// <exception cref="NotImplementedException">not implemented yet</exception>
        public string TPCreditCardLookup(string source, int creditCardTypeId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The credit card lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="creditCard">The credit card.</param>
        /// <returns>
        /// credit card
        /// </returns>
        /// <exception cref="NotImplementedException">not implemented yet</exception>
        public int TPCreditCardLookup(string source, string creditCard)
        {
            throw new NotImplementedException();
        }

        /// <summary>The currency lookup.</summary>
        /// <param name="source">The source.</param>
        /// <param name="currencyCode">The currency identifier.</param>
        /// <returns>The currency</returns>
        /// <exception cref="NotImplementedException">not implemented</exception>
        public string TPCurrencyLookup(string source, string currencyCode)
        {
            return this.TPCurrency(source).FirstOrDefault(c => c.Key == currencyCode).Value;
        }

        /// <summary>
        /// The extra type lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="extraTypeID">The extra type identifier.</param>
        /// <returns>
        /// extra type
        /// </returns>
        /// <exception cref="NotImplementedException">not implemented yet</exception>
        public string TPExtraTypeLookup(string source, int extraTypeID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The extra type lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="extraType">Type of the extra.</param>
        /// <returns>
        /// extra type
        /// </returns>
        /// <exception cref="NotImplementedException">not implemented yet</exception>
        public int TPExtraTypeLookup(string source, string extraType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The flight class lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="flightClassID">The flight class identifier.</param>
        /// <returns>
        /// flight class
        /// </returns>
        /// <exception cref="NotImplementedException">not implemented yet</exception>
        public string TPFlightClassLookup(string source, int flightClassID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The flight class lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="flightClass">The flight class.</param>
        /// <returns>
        /// flight class
        /// </returns>
        /// <exception cref="NotImplementedException">not implemented yet</exception>
        public int TPFlightClassLookup(string source, string flightClass)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The interface lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        /// The Third Party Interface
        /// </returns>
        /// <exception cref="NotImplementedException">not implemented yet</exception>
        public string TPInterfaceLookup(string source)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The language code lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="languageID">The language identifier.</param>
        /// <returns>
        /// The third party language code
        /// </returns>
        /// <exception cref="NotImplementedException">not implemented yet</exception>
        public string TPLanguageCodeLookup(string source, int languageID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The language code lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="languageCode">The language code.</param>
        /// <returns>
        /// language code
        /// </returns>
        /// <exception cref="NotImplementedException">not implemented yet</exception>
        public int TPLanguageCodeLookup(string source, string languageCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The meal bases.
        /// </summary>
        /// <param name="source">The s source.</param>
        /// <returns>
        /// meal bases
        /// </returns>
        /// <exception cref="NotImplementedException">not implemented yet</exception>
        public Dictionary<string, int> TPMealBases(string source)
        {
            return this.TPMealBasis(source);
        }

        /// <summary>
        /// The meal basis lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="mealBasisID">The meal basis identifier.</param>
        /// <returns>
        /// meal basis
        /// </returns>
        /// <exception cref="NotImplementedException">not implemented yet</exception>
        public string TPMealBasisLookup(string source, int mealBasisID)
        {
            return this.TPMealBasis(source).FirstOrDefault(c => c.Value == mealBasisID).Key;
        }

        /// <summary>
        /// The meal basis lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="mealBasis">The meal basis.</param>
        /// <returns>
        /// meal basis
        /// </returns>
        /// <exception cref="NotImplementedException">not implemented yet</exception>
        public int TPMealBasisLookup(string source, string mealBasis)
        {
            return this.TPMealBasis(source).FirstOrDefault(c => c.Key == mealBasis).Value;
        }

        /// <summary>
        /// The nationality lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="nationalityID">The nationality identifier.</param>
        /// <returns>
        /// The Nationality
        /// </returns>
        /// <exception cref="NotImplementedException">not implemented yet</exception>
        public string TPNationalityLookup(string source, int nationalityID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The nationality lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="nationality">The nationality.</param>
        /// <returns>
        /// The third party nationality code or null if not found 
        /// </returns>
        public string TPNationalityLookup(string source, string nationality)
        {
            nationality = Regex.Replace(nationality, @"\s+", string.Empty); // remove all line breaks and whitespaces 
            return this.TPNationality(source).FirstOrDefault(c => c.Key == nationality).Value;
        }
        /// <summary>
        /// The offsite payment type lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="offsitePaymentTypeID">The offsite payment type identifier.</param>
        /// <returns>
        /// offsite payment type
        /// </returns>
        /// <exception cref="NotImplementedException">not implemented yet</exception>
        public string TPOffsitePaymentTypeLookup(string source, int offsitePaymentTypeID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The resort lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="resort">The resort.</param>
        /// <returns>
        /// The resort ID
        /// </returns>
        /// <exception cref="NotImplementedException">not implemented yet</exception>
        public int TPResortLookup(string source, string resort)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the station code.
        /// </summary>
        /// <param name="source">The s source.</param>
        /// <param name="stationID">The i station identifier.</param>
        /// <returns>
        /// Station code
        /// </returns>
        /// <exception cref="NotImplementedException">not implemented yet</exception>
        public string TPStationCodeLookup(string source, int stationID)
        {
            throw new NotImplementedException();
        }

        /// <summary>Third party currency lookup.</summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        private Dictionary<string, string> TPCurrency(string source)
        {
            string cacheKey = "TPCurrencyLookup_" + source;

            async Task<Dictionary<string, string>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select ThirdPartyCurrencyCode, CurrencyCode from Currency where Source = @source",
                    async r => (await r.ReadAllAsync<Currency>()).ToDictionary(x => x.CurrencyCode, x => x.ThirdPartyCurrencyCode),
                    new CommandSettings().WithParameters(new { source }));
            }

            // todo - make async
            return _cache.GetOrCreateAsync(cacheKey, cacheBuilder, 60).Result;
        }

        private class Currency
        {
            public string ThirdPartyCurrencyCode { get; set; }
            public string CurrencyCode { get; set; }
        }

        /// <summary>
        ///   <para>third party meal basis lookup</para>
        /// </summary>
        /// <param name="source">The s source.</param>
        /// <returns>
        ///   a dictionary of meal basis code and identifier
        /// </returns>
        private Dictionary<string, int> TPMealBasis(string source)
        {
            string cacheKey = "TPMealBasisLookup_" + source;

            async Task<Dictionary<string, int>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select MealBasisCode, MealBasisID from Mealbasis where Source = @source",
                    async r => (await r.ReadAllAsync<MealBasis>()).ToDictionary(x => x.MealBasisCode, x => x.MealBasisID),
                    new CommandSettings().WithParameters(new { source }));
            }

            // todo - make async
            return _cache.GetOrCreateAsync(cacheKey, cacheBuilder, 60).Result;
        }

        private class MealBasis
        {
            public string MealBasisCode { get; set; }
            public int MealBasisID { get; set; }
        }

        /// <summary>Booking country lookup</summary>
        /// <param name="source">The source.</param>
        /// <returns>a dictionary of third party booking country and booking country identifier</returns>
        private Dictionary<string, int> TPBookingCountryCode(string source)
        {
            string cacheKey = "TPBookingCountryCodeLookup_" + source;

            async Task<Dictionary<string, int>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select TPBookingCountryCode, BookingCountryLookupID from BookingCountryLookup where Source = @source",
                    async r => (await r.ReadAllAsync<BookingCountry>()).ToDictionary(x => x.TPBookingCountryCode, x => x.BookingCountryLookupID),
                    new CommandSettings().WithParameters(new { source }));
            }

            // todo - make async
            return _cache.GetOrCreateAsync(cacheKey, cacheBuilder, 60).Result;
        }

        private class BookingCountry
        {
            public string TPBookingCountryCode { get; set; }
            public int BookingCountryLookupID { get; set; }
        }

        /// <summary>Nationality lookup</summary>
        /// <param name="source">The source.</param>
        /// <returns>a dictionary of third party national identifier and corrsponding ISO code</returns>
        private Dictionary<string, string> TPNationality(string source)
        {
            string cacheKey = "TPNationalityLookup_" + source;

            async Task<Dictionary<string, string>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select NationalityCode, ISOCode from NationalityLookup where Source = @source",
                    async r => (await r.ReadAllAsync<Nationality>()).ToDictionary(x => x.NationalityCode, x => x.NationalityLookup),
                    new CommandSettings().WithParameters(new { source }));
            }

            // todo - make async
            return _cache.GetOrCreateAsync(cacheKey, cacheBuilder, 60).Result;
        }

        private class Nationality
        {
            public string NationalityCode { get; set; }
            public string NationalityLookup { get; set; }
        }

        /// <summary>ISO currency id lookup.</summary>
        /// <param name="currencyCode">The ISO currency code.</param>
        /// <returns>The currency id of the corresponding currencyCode</returns>
        public async Task<int> TPCurrencyIDLookupAsync(string currencyCode)
        {
            // todo - caching
            return await _sql.ReadScalarAsync<int>(
                "select ISOCurrencyID from ISOCurrency where CurrencyCode = @currencyCode",
                new CommandSettings().WithParameters(new { currencyCode }));
        }

        /// <summary>ISO currency code lookup.</summary>
        /// <param name="currencyId">The ISO currency id.</param>
        /// <returns>The currency code of corresponding currency id</returns>
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
        private Dictionary<string, string> TPCountryCode(string source)
        {
            string cacheKey = "TPCountyCodeLookup_" + source;

            async Task<Dictionary<string, string>> cacheBuilder()
            {
                return await _sql.ReadSingleMappedAsync(
                    "select ISOCode, TPCountryCode from CountryCodeLookup where Source = @source",
                    async r => (await r.ReadAllAsync<Country>()).ToDictionary(x => x.ISOCode, x => x.TPCountryCode),
                    new CommandSettings().WithParameters(new { source }));
            }

            // todo - make async
            return _cache.GetOrCreateAsync(cacheKey, cacheBuilder, 60).Result;
        }

        private class Country
        {
            public string ISOCode { get; set; }
            public string TPCountryCode { get; set; }
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
            public string Code { get; set; }
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
            public string Code { get; set; }
        }
    }
}