using System.Collections.Concurrent;
using System.Threading.Tasks;
using iVectorOne.Models.SearchStore;
using iVectorOne.Repositories;

namespace iVectorOne.Services
{
    public class SearchStoreService : ISearchStoreService
    {
        private readonly int _bulkInsertSize;
        private ConcurrentBag<SearchStoreItem> _searchStoreItems = new();
        private ConcurrentBag<SearchStoreSupplierItem> _searchStoreSupplierItems = new();

        private readonly ISearchStoreRepository _searchStoreRepository;

        public SearchStoreService(ISearchStoreRepository searchStoreRepository, int bulkInsertSize)
        {
            _searchStoreRepository = searchStoreRepository;
            _bulkInsertSize = bulkInsertSize;
        }

        public async Task AddAsync(SearchStoreItem item)
        {
            _searchStoreItems.Add(item);

            if (_searchStoreItems.Count >= _bulkInsertSize)
            {
                ConcurrentBag<SearchStoreItem> items = null!;

                lock (_searchStoreItems)
                {
                    if (_searchStoreItems.Count >= _bulkInsertSize)
                    {
                        items = _searchStoreItems;
                        _searchStoreItems = new ConcurrentBag<SearchStoreItem>();
                    }
                }

                if (items != null)
                {
                    await _searchStoreRepository.BulkInsertAsync(items);
                }
            }
        }

        public async Task AddAsync(SearchStoreSupplierItem item)
        {
            _searchStoreSupplierItems.Add(item);

            if (_searchStoreSupplierItems.Count >= _bulkInsertSize)
            {
                ConcurrentBag<SearchStoreSupplierItem> items = null!;

                lock (_searchStoreSupplierItems)
                {
                    if (_searchStoreSupplierItems.Count >= _bulkInsertSize)
                    {
                        items = _searchStoreSupplierItems;
                        _searchStoreSupplierItems = new ConcurrentBag<SearchStoreSupplierItem>();
                    }
                }

                if (items != null)
                {
                    await _searchStoreRepository.BulkInsertAsync(items);
                }
            }
        }
    }
}
