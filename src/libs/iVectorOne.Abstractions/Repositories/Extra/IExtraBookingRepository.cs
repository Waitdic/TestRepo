namespace iVectorOne.Repositories
{
    using iVectorOne.Models.Extra;
    using System.Threading.Tasks;

    public interface IExtraBookingRepository
    {
        Task<int> StoreBookingAsync(ExtraDetails extraDetails, bool requestValid, bool success);
    }
}