namespace ThirdParty.CSSuppliers.ExpediaRapid
{
    using Intuitive.Net.WebRequests;
    using System.Threading.Tasks;
    using ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses;
    using ThirdParty.Models.Property.Booking;

    public interface IExpediaRapidAPI
    {
        Task<TResponse> GetDeserializedResponseAsync<TResponse>(PropertyDetails propertyDetails, Request request) where TResponse : IExpediaRapidResponse<TResponse>, new();
        Task<string> GetResponseAsync(PropertyDetails propertyDetails, Request request);
    }
}