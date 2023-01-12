namespace iVectorOne.Repositories
{
    using System.Threading.Tasks;
    using iVectorOne.Models.Transfer;

    public interface ITransferSupplierLogRepository
    {
        Task LogPrebookRequestsAsync(TransferDetails transferDetails);
        Task LogBookRequestsAsync(TransferDetails transferDetails);
        Task LogPrecancelRequestsAsync(TransferDetails transferDetails);
        Task LogCancelRequestsAsync(TransferDetails transferDetails);
    }
}