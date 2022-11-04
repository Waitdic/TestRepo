using System.Collections.Generic;
using System.Threading.Tasks;
using iVectorOne.Models.SearchStore;

namespace iVectorOne.Repositories
{
    public interface ISearchStoreRepository
    {
        Task BulkInsertAsync(IEnumerable<SearchStoreItem> searchStoreItems);

        Task BulkInsertAsync(IEnumerable<SearchStoreSupplierItem> searchStoreSupplierItems);
    }
}
