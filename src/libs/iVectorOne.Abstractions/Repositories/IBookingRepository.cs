namespace iVectorOne.Repositories
{
    using iVectorOne.Models.Property.Booking;
    using System.Threading.Tasks;

    public interface IBookingRepository
    {
        Task<int> StoreBookingAsync(PropertyDetails propertyDetails, bool requestValid, bool success);
    }
}