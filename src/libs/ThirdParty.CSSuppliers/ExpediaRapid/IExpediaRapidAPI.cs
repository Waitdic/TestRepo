namespace ThirdParty.CSSuppliers.ExpediaRapid
{
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses;

    public interface IExpediaRapidAPI
    {
        TResponse GetDeserializedResponse<TResponse>(PropertyDetails propertyDetails, Intuitive.Net.WebRequests.Request request) where TResponse : IExpediaRapidResponse, new();
        string GetResponse(PropertyDetails propertyDetails, Intuitive.Net.WebRequests.Request request);
    }

}