namespace iVectorOne.Repositories
{
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;
    using System.Threading.Tasks;

    public interface IBookingRepository
    {
        Task<int> StoreBookingAsync(PropertyDetails propertyDetails, bool requestValid, bool success);
        Task<Booking?> GetBookingAsync(string bookingReference, string supplierBookingReference, Account account);
        Task<Booking?> GetBookingAsync(string supplierBookingReference, Account account);
        Task<int> StoreBookingAsync(Booking booking);
    }
}