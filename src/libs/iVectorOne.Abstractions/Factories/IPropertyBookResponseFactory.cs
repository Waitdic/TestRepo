namespace iVectorOne.Factories
{
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.SDK.V2.PropertyBook;

    /// <summary>A factory that creates property book responses using the provided property details</summary>
    public interface IPropertyBookResponseFactory
    {
        /// <summary>
        /// Creates a book response using information from the property details
        /// </summary>
        /// <param name="propertyDetails">The property details which contain all the information from the third party pre book</param>
        /// <returns>A book response</returns>
        Response Create(PropertyDetails propertyDetails);
    }
}