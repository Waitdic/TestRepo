namespace iVectorOne.Services.Extra
{
    using System.Threading.Tasks;
    using iVectorOne.Models.SearchStore;
    public interface ISearchStoreService
    {
        Task AddAsync(ExtraSearchStoreItem searchStoreItem);

        Task AddAsync(ExtraSearchStoreSupplierItem searchStoreSupplierItem);
    }
}
