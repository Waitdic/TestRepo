using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using iVectorOne.Models.SearchStore;
using Microsoft.Data.SqlClient;

namespace iVectorOne.Repositories
{
    public class SearchStoreRepository : ISearchStoreRepository
    {
        private readonly string _connectionString;

        public SearchStoreRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task BulkInsertAsync(IEnumerable<SearchStoreItem> searchStoreItems)
        {
            using var sqlBulkCopy = new SqlBulkCopy(_connectionString);
            sqlBulkCopy.DestinationTableName = "dbo.SearchStore";
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreItem.SearchStoreId), "SearchStoreID");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreItem.AccountName), "AccountName");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreItem.AccountId), "AccountID");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreItem.System), "System");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreItem.Successful), "Successful");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreItem.SearchDateAndTime), "SearchDateAndTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreItem.PropertiesRequested), "PropertiesRequested");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreItem.PropertiesReturned), "PropertiesReturned");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreItem.PreProcessTime), "PreProcessTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreItem.MaxSupplierTime), "MaxSupplierTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreItem.MaxDedupeTime), "MaxDedupeTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreItem.PostProcessTime), "PostProcessTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreItem.TotalTime), "TotalTime");

            var table = new DataTable();
            table.Columns.AddRange(
                new DataColumn[]{
                new("SearchStoreID", typeof(Guid)),
                new("AccountName"),
                new("AccountID"),
                new("System"),
                new("Successful"),
                new("SearchDateAndTime"),
                new("PropertiesRequested"),
                new("PropertiesReturned"),
                new("PreProcessTime"),
                new("MaxSupplierTime"),
                new("MaxDedupeTime"),
                new("PostProcessTime"),
                new("TotalTime")
            });

            foreach (var item in searchStoreItems)
            {
                table.Rows.Add(
                    item.SearchStoreId,
                    item.AccountName,
                    item.AccountId,
                    item.System,
                    item.Successful,
                    item.SearchDateAndTime,
                    item.PropertiesRequested,
                    item.PropertiesReturned,
                    item.PreProcessTime,
                    item.MaxSupplierTime,
                    item.MaxDedupeTime,
                    item.PostProcessTime,
                    item.TotalTime);
            }

            await sqlBulkCopy.WriteToServerAsync(table);
        }

        public async Task BulkInsertAsync(IEnumerable<SearchStoreSupplierItem> searchStoreSupplierItems)
        {
            using var sqlBulkCopy = new SqlBulkCopy(_connectionString);
            sqlBulkCopy.DestinationTableName = "dbo.SearchStoreSupplier";
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreSupplierItem.SearchStoreId), "SearchStoreID");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreSupplierItem.AccountName), "AccountName");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreSupplierItem.AccountId), "AccountID");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreSupplierItem.System), "System");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreSupplierItem.SupplierName), "SupplierName");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreSupplierItem.SupplierId), "SupplierID");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreSupplierItem.Successful), "Successful");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreSupplierItem.Timeout), "Timeout");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreSupplierItem.SearchDateAndTime), "SearchDateAndTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreSupplierItem.PropertiesRequested), "PropertiesRequested");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreSupplierItem.PropertiesReturned), "PropertiesReturned");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreSupplierItem.PreProcessTime), "PreProcessTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreSupplierItem.SupplierTime), "SupplierTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreSupplierItem.DedupeTime), "DedupeTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreSupplierItem.PostProcessTime), "PostProcessTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(SearchStoreSupplierItem.TotalTime), "TotalTime");

            var table = new DataTable();
            table.Columns.AddRange(
                new DataColumn[]
                {
                    new("SearchStoreID", typeof(Guid)),
                    new("AccountName"),
                    new("AccountID"),
                    new("System"),
                    new("SupplierName"),
                    new("SupplierId"),
                    new("Successful"),
                    new("Timeout"),
                    new("SearchDateAndTime"),
                    new("PropertiesRequested"),
                    new("PropertiesReturned"),
                    new("PreProcessTime"),
                    new("SupplierTime"),
                    new("DedupeTime"),
                    new("PostProcessTime"),
                    new("TotalTime")
                });

            foreach (var item in searchStoreSupplierItems)
            {
                table.Rows.Add(
                    item.SearchStoreId,
                    item.AccountName,
                    item.AccountId,
                    item.System,
                    item.SupplierName,
                    item.SupplierId,
                    item.Successful,
                    item.Timeout,
                    item.SearchDateAndTime,
                    item.PropertiesRequested,
                    item.PropertiesReturned,
                    item.PreProcessTime,
                    item.SupplierTime,
                    item.DedupeTime,
                    item.PostProcessTime,
                    item.TotalTime);
            }

            await sqlBulkCopy.WriteToServerAsync(table);
        }
    }
}
