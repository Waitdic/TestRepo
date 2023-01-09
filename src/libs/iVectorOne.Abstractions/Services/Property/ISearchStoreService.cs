using System.Threading.Tasks;
using iVectorOne.Models.SearchStore;

namespace iVectorOne.Services
{
    public interface ISearchStoreService
    {
        Task AddAsync(SearchStoreItem searchStoreItem);

        Task AddAsync(SearchStoreSupplierItem searchStoreSupplierItem);
    }
}
