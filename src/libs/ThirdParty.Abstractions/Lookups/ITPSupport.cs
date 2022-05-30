namespace ThirdParty.Lookups
{
    using System.Collections.Generic;

    /// <summary>
    /// The support functions for third parties
    /// </summary>
    public interface ITPSupport
    {
        /// <summary>
        /// Returns the station code.
        /// </summary>
        /// <param name="source">The s source.</param>
        /// <param name="stationID">The i station identifier.</param>
        /// <returns>Station code</returns>
        string TPStationCodeLookup(string source, int stationID);

        /// <summary>
        /// Returns the booking country.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="bookingCountryID">The booking country identifier.</param>
        /// <returns>Booking country</returns>
        string TPBookingCountryLookup(string source, int bookingCountryID);

        /// <summary>
        /// The country code lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="countryCode">The iso country code.</param>
        /// <returns>
        /// The third party country code
        /// </returns>
        string TPCountryCodeLookup(string source, string countryCode);

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
        string TPCurrencyLookup(string source, string currencyCode);

        /// <summary>
        /// The flight class lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="flightClassID">The flight class identifier.</param>
        /// <returns>flight class</returns>
        string TPFlightClassLookup(string source, int flightClassID);

        /// <summary>
        /// The language code lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="languageID">The language identifier.</param>
        /// <returns>The third party language code</returns>
        string TPLanguageCodeLookup(string source, int languageID);

        /// <summary>
        /// The meal basis lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="mealBasisID">The meal basis identifier.</param>
        /// <returns>meal basis</returns>
        string TPMealBasisLookup(string source, int mealBasisID);

        /// <summary>
        /// The nationality lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="nationalityID">The nationality identifier.</param>
        /// <returns>The Nationality</returns>
        string TPNationalityLookup(string source, int nationalityID);

        /// <summary>
        /// The offsite payment type lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="offsitePaymentTypeID">The offsite payment type identifier.</param>
        /// <returns>offsite payment type</returns>
        string TPOffsitePaymentTypeLookup(string source, int offsitePaymentTypeID);

        /// <summary>
        /// Currencies lookup.
        /// </summary>
        /// <param name="currencyID">The currency identifier.</param>
        /// <returns>The currency that matches the provided currencyID</returns>
        string CurrencyLookup(int currencyID);

        /// <summary>
        /// The interface lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The Third Party Interface</returns>
        string TPInterfaceLookup(string source);

        /// <summary>
        /// The extra type lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="extraTypeID">The extra type identifier.</param>
        /// <returns>extra type</returns>
        string TPExtraTypeLookup(string source, int extraTypeID);

        /// <summary>
        /// The resort lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="resort">The resort.</param>
        /// <returns>The resort ID</returns>
        int TPResortLookup(string source, string resort);

        /// <summary>
        /// The booking country code lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="bookingCountryCode">The booking country code.</param>
        /// <returns>booking country code</returns>
        int TPBookingCountryCodeLookup(string source, string bookingCountryCode);

        /// <summary>
        /// The credit card lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="creditCard">The credit card.</param>
        /// <returns>credit card</returns>
        int TPCreditCardLookup(string source, string creditCard);

        /// <summary>
        /// The flight class lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="flightClass">The flight class.</param>
        /// <returns>flight class</returns>
        int TPFlightClassLookup(string source, string flightClass);

        /// <summary>
        /// The language code lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="languageCode">The language code.</param>
        /// <returns>language code </returns>
        int TPLanguageCodeLookup(string source, string languageCode);

        /// <summary>
        /// The meal basis lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="mealBasis">The meal basis.</param>
        /// <returns>meal basis</returns>
        int TPMealBasisLookup(string source, string mealBasis);

        /// <summary>
        /// The nationality lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="nationality">The nationality.</param>
        /// <returns>The Nationality</returns>
        string TPNationalityLookup(string source, string nationality);

        /// <summary>
        /// The extra type lookup.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="extraType">Type of the extra.</param>
        /// <returns>extra type</returns>
        int TPExtraTypeLookup(string source, string extraType);

        /// <summary>
        /// The meal bases.
        /// </summary>
        /// <param name="source">The s source.</param>
        /// <returns>meal bases</returns>
        Dictionary<string, int> TPMealBases(string source);

        /// <summary>
        /// The resort code lookup by property id.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="propertyId">The property id.</param>
        /// <returns>The resort ID</returns>
        string TPResortCodeByPropertyIdLookup(string source, int propertyId);

        /// <summary>
        /// The resort code lookup by geography id.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="geographyId">The geography id.</param>
        /// <returns>The resort ID</returns>
        string TPResortCodeByGeographyIdLookup(string source, int geographyId);
        int TPCurrencyIDLookup(string currencyCode);
        string TPCurrencyCodeLookup(int currencyID);
    }
}