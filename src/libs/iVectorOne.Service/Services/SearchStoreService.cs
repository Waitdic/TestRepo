using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Intuitive;
using iVectorOne.Models.SearchStore;
using iVectorOne.Repositories;
using Microsoft.Extensions.Logging;

namespace iVectorOne.Services
{
    public class SearchStoreService : ISearchStoreService, IAsyncDisposable
    {
        private readonly ILogger<SearchStoreService> _logger;
        private readonly int _bulkInsertSize;
        private ConcurrentBag<SearchStoreItem> _searchStoreItems = new();
        private ConcurrentBag<SearchStoreSupplierItem> _searchStoreSupplierItems = new();

        private readonly ISearchStoreRepository _searchStoreRepository;

        public SearchStoreService(ILogger<SearchStoreService> logger, ISearchStoreRepository searchStoreRepository, int bulkInsertSize)
        {
            _logger = Ensure.IsNotNull(logger, nameof(logger));
            _searchStoreRepository = Ensure.IsNotNull(searchStoreRepository, nameof(searchStoreRepository)); ;
            _bulkInsertSize = bulkInsertSize;
        }

        public async Task AddAsync(SearchStoreItem item)
        {
            try
            {
                _searchStoreItems.Add(item);

                if (_searchStoreItems.Count >= _bulkInsertSize)
                {
                    await Task.Run(async () => await FlushSearchStoreAsync(_bulkInsertSize));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the entry SearchStoreItem");
            }
        }

        public async Task AddAsync(SearchStoreSupplierItem item)
        {
            try
            {
                _searchStoreSupplierItems.Add(item);

                if (_searchStoreSupplierItems.Count >= _bulkInsertSize)
                {
                    await Task.Run(async () => await FlushSearchStoreSupplierAsync(_bulkInsertSize));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the entry SearchStoreSupplierItem");
            }
        }

        private async Task FlushSearchStoreAsync(int bulkInsertSize)
        {

            ConcurrentBag<SearchStoreItem> items = null!;

            lock (_searchStoreItems)
            {
                if (_searchStoreItems.Count >= bulkInsertSize)
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

        private async Task FlushSearchStoreSupplierAsync(int bulkInsertSize)
        {
            ConcurrentBag<SearchStoreSupplierItem> items = null!;

            lock (_searchStoreSupplierItems)
            {
                if (_searchStoreSupplierItems.Count >= bulkInsertSize)
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

        public async ValueTask DisposeAsync()
        {
            await FlushSearchStoreAsync(1);
            await FlushSearchStoreSupplierAsync(1);
        }
    }
}
