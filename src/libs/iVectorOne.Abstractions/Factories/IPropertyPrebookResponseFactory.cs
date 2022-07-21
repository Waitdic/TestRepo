namespace ThirdParty.Factories
{
    using System.Threading.Tasks;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.SDK.V2.PropertyPrebook;

    /// <summary>A factory that creates property pre book responses using the provided property details</summary>
    public interface IPropertyPrebookResponseFactory
    {
        /// <summary>Creates a pre book response using information from the property details</summary>
        /// <param name="propertyDetails">The property details which contain all the information from the third party pre book</param>
        /// <returns>A pre book response</returns>
        Task<Response> CreateAsync(PropertyDetails propertyDetails);
    }
}