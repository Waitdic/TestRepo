namespace iVectorOne.Services.Transfer
{
    using System.Threading.Tasks;
    using iVectorOne.Models.SearchStore;
    public interface ISearchStoreService
    {
        Task AddAsync(TransferSearchStoreItem searchStoreItem);

        Task AddAsync(TransferSearchStoreSupplierItem searchStoreSupplierItem);
    }
}
