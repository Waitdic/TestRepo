namespace ThirdParty.CSSuppliers.ExpediaRapid
{
    using System.Threading.Tasks;
    using Intuitive.Helpers.Net;
    using ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses;
    using ThirdParty.Models.Property.Booking;

    public interface IExpediaRapidAPI
    {
        Task<TResponse> GetDeserializedResponseAsync<TResponse>(PropertyDetails propertyDetails, Request request) where TResponse : IExpediaRapidResponse<TResponse>, new();
        Task<string> GetResponseAsync(PropertyDetails propertyDetails, Request request);
    }
}