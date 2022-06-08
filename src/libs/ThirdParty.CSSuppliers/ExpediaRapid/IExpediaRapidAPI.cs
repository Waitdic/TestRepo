namespace ThirdParty.CSSuppliers.ExpediaRapid
{
    using Intuitive.Net.WebRequests;
    using ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses;
    using ThirdParty.Models.Property.Booking;

    public interface IExpediaRapidAPI
    {
        TResponse GetDeserializedResponse<TResponse>(PropertyDetails propertyDetails, Request request) where TResponse : IExpediaRapidResponse<TResponse>, new();
        string GetResponse(PropertyDetails propertyDetails, Request request);
    }
}