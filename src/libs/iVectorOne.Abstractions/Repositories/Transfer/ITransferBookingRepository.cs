namespace iVectorOne.Repositories
{
    using iVectorOne.Models.Transfer;
    using System.Threading.Tasks;

    public interface ITransferBookingRepository
    {
        Task<int> StoreBookingAsync(TransferDetails transferDetails, bool requestValid, bool success);
    }
}