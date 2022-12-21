using System.Collections.Generic;
using System.Threading.Tasks;
using iVectorOne.Models.SearchStore;

namespace iVectorOne.Repositories
{
    public interface IExtraSearchStoreRepository
    {
        Task BulkInsertAsync(IEnumerable<ExtraSearchStoreItem> searchStoreItems);

        Task BulkInsertAsync(IEnumerable<ExtraSearchStoreSupplierItem> searchStoreSupplierItems);
    }
}
