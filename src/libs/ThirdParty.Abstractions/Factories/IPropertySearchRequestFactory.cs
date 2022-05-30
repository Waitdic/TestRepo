namespace ThirdParty.Factories
{
    using System;
    using ThirdParty.SDK.V2.PropertySearch;

    /// <summary>
    /// an interface defining a property search request factory, that takes the required data and builds a property search request
    /// </summary>
    public interface IPropertySearchRequestFactory
    {
        /// <summary>
        /// Creates the specified properties.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="rooms">The rooms.</param>
        /// <param name="arrivalDate">The arrival date.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="nationalityID">The unique nationality identifier.</param>
        /// <param name="currencyCode">The curreny code</param>
        /// <param name="opaqueRates">a boolean that decides if opaque rates are supported</param>
        /// <param name="sellingCountry">The selling country</param>
        /// <returns>a property search request</returns>
        Request Create(string properties, string rooms, DateTime? arrivalDate, int duration, string nationalityID, string currencyCode, bool opaqueRates, string sellingCountry);
    }
}