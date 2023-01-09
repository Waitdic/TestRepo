using System.Collections.Generic;
using System.Threading.Tasks;
using iVectorOne.Models.SearchStore;

namespace iVectorOne.Repositories
{
    public interface ITransferSearchStoreRepository
    {
        Task BulkInsertAsync(IEnumerable<TransferSearchStoreItem> searchStoreItems);

        Task BulkInsertAsync(IEnumerable<TransferSearchStoreSupplierItem> searchStoreSupplierItems);
    }
}
