namespace iVectorOne.Repositories
{
    using System.Threading.Tasks;
    using iVectorOne.Models.Property.Booking;

    public interface ISupplierLogRepository
    {
        Task LogPrebookRequestsAsync(PropertyDetails propertyDetails);
        Task LogBookRequestsAsync(PropertyDetails propertyDetails);
        Task LogPrecancelRequestsAsync(PropertyDetails propertyDetails);
        Task LogCancelRequestsAsync(PropertyDetails propertyDetails);
    }
}