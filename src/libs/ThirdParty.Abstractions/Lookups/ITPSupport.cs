namespace ThirdParty.Lookups
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The support functions for third parties
    /// </summary>
    public interface ITPSupport
    {
        /// <summary>
        /// Returns the booking country.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="bookingCountryId">The booking country identifier.</param>
        /// <returns>Booking country</returns>
        Task<string> TPBookingCountryLookupAsync(string source, int bookingCountryId);

        /// <summary>
        /// The country code lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="countryCode">The iso country code.</param>
        /// <returns>
        /// The third party country code
        /// </returns>
        Task<string> TPCountryCodeLookupAsync(string source, string countryCode);

        /// <summary>
        /// The credit card lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="creditCardTypeId">The credit card type identifier.</param>
        /// <returns>credit card</returns>
        string TPCreditCardLookup(string source, int creditCardTypeId);

        /// <summary>
        /// The currency lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="currencyCode">The ISO currency code.</param>
        /// <returns>The currency</returns>
        Task<string> TPCurrencyLookupAsync(string source, string currencyCode);

        /// <summary>
        /// The meal basis lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="mealBasisId">The meal basis identifier.</param>
        /// <returns>meal basis</returns>
        Task<string> TPMealBasisLookupAsync(string source, int mealBasisId);

        /// <summary>
        /// Currencies lookup.
        /// </summary>
        /// <param name="currencyId">The currency identifier.</param>
        /// <returns>The currency that matches the provided currencyID</returns>
        string CurrencyLookup(int currencyId);

        /// <summary>
        /// The booking country code lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="bookingCountryCode">The booking country code.</param>
        /// <returns>booking country code</returns>
        Task<int> TPBookingCountryCodeLookupAsync(string source, string bookingCountryCode);

        /// <summary>
        /// The meal basis lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="mealBasis">The meal basis.</param>
        /// <returns>meal basis</returns>
        Task<int> TPMealBasisLookupAsync(string source, string mealBasis);

        /// <summary>
        /// The nationality lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="nationality">The nationality.</param>
        /// <returns>The Nationality</returns>
        Task<string> TPNationalityLookupAsync(string source, string nationality);

        /// <summary>
        /// The meal bases.
        /// </summary>
        /// <param name="source">The s source.</param>
        /// <returns>meal bases</returns>
        Task<Dictionary<string, int>> TPMealBasesAsync(string source);

        /// <summary>
        /// The resort code lookup by property id.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="propertyId">The property id.</param>
        /// <returns>The resort ID</returns>
        Task<string> TPResortCodeByPropertyIdLookupAsync(string source, int propertyId);

        /// <summary>
        /// The resort code lookup by geography id.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="geographyId">The geography id.</param>
        /// <returns>The resort ID</returns>
        Task<string> TPResortCodeByGeographyIdLookupAsync(string source, int geographyId);

        /// <summary>ISO currency id lookup.</summary>
        /// <param name="currencyCode">The ISO currency code.</param>
        /// <returns>The currency id of the corresponding currencyCode</returns>
        Task<int> TPCurrencyIDLookupAsync(string currencyCode);

        /// <summary>ISO currency code lookup.</summary>
        /// <param name="currencyId">The ISO currency id.</param>
        /// <returns>The currency code of corresponding currency id</returns>
        Task<string> TPCurrencyCodeLookupAsync(int currencyId);
    }
}