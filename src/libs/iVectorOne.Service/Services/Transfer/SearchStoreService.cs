namespace iVectorOne.Services.Transfer
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Intuitive;
    using iVectorOne.Models.SearchStore;
    using iVectorOne.Repositories;
    using Microsoft.Extensions.Logging;

    public class SearchStoreService : ISearchStoreService, IAsyncDisposable
    {
        private readonly ILogger<SearchStoreService> _logger;
        private readonly int _bulkInsertSize;
        private ConcurrentBag<TransferSearchStoreItem> _searchStoreItems = new();
        private ConcurrentBag<TransferSearchStoreSupplierItem> _searchStoreSupplierItems = new();

        private readonly ITransferSearchStoreRepository _searchStoreRepository;

        public SearchStoreService(ILogger<SearchStoreService> logger, ITransferSearchStoreRepository searchStoreRepository, int bulkInsertSize)
        {
            _logger = Ensure.IsNotNull(logger, nameof(logger));
            _searchStoreRepository = Ensure.IsNotNull(searchStoreRepository, nameof(searchStoreRepository)); ;
            _bulkInsertSize = bulkInsertSize;
        }

        public async Task AddAsync(TransferSearchStoreItem item)
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

        public async Task AddAsync(TransferSearchStoreSupplierItem item)
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
            ConcurrentBag<TransferSearchStoreItem> items = null!;

            lock (_searchStoreItems)
            {
                if (_searchStoreItems.Count >= bulkInsertSize)
                {
                    items = _searchStoreItems;
                    _searchStoreItems = new ConcurrentBag<TransferSearchStoreItem>();
                }
            }

            if (items != null)
            {
                await _searchStoreRepository.BulkInsertAsync(items);
            }
        }

        private async Task FlushSearchStoreSupplierAsync(int bulkInsertSize)
        {
            ConcurrentBag<TransferSearchStoreSupplierItem> items = null!;

            lock (_searchStoreSupplierItems)
            {
                if (_searchStoreSupplierItems.Count >= bulkInsertSize)
                {
                    items = _searchStoreSupplierItems;
                    _searchStoreSupplierItems = new ConcurrentBag<TransferSearchStoreSupplierItem>();
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