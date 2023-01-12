namespace iVectorOne.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using iVectorOne.Models.SearchStore;
    using Microsoft.Data.SqlClient;

    public class TransferSearchStoreRepository : ITransferSearchStoreRepository
    {
        private readonly string _connectionString;

        public TransferSearchStoreRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task BulkInsertAsync(IEnumerable<TransferSearchStoreItem> searchStoreItems)
        {
            using var sqlBulkCopy = new SqlBulkCopy(_connectionString);
            sqlBulkCopy.DestinationTableName = "dbo.TransferSearchStoreApi";
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreItem.TransferSearchStoreId), "TransferSearchStoreID");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreItem.AccountName), "AccountName");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreItem.AccountId), "AccountID");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreItem.System), "System");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreItem.Successful), "Successful");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreItem.SearchDateAndTime), "SearchDateAndTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreItem.ResultsReturned), "ResultsReturned");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreItem.PreProcessTime), "PreProcessTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreItem.MaxSupplierTime), "MaxSupplierTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreItem.PostProcessTime), "PostProcessTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreItem.TotalTime), "TotalTime");

            var table = new DataTable();
            table.Columns.AddRange(
                new DataColumn[]{
                new("TransferSearchStoreID", typeof(Guid)),
                new("AccountName"),
                new("AccountID"),
                new("System"),
                new("Successful"),
                new("SearchDateAndTime"),
                new("ResultsReturned"),
                new("PreProcessTime"),
                new("MaxSupplierTime"),
                new("PostProcessTime"),
                new("TotalTime")
            });

            foreach (var item in searchStoreItems)
            {
                table.Rows.Add(
                    item.TransferSearchStoreId,
                    item.AccountName,
                    item.AccountId,
                    item.System,
                    item.Successful,
                    item.SearchDateAndTime,
                    item.ResultsReturned,
                    item.PreProcessTime,
                    item.MaxSupplierTime,
                    item.PostProcessTime,
                    item.TotalTime);
            }

            await sqlBulkCopy.WriteToServerAsync(table);
        }

        public async Task BulkInsertAsync(IEnumerable<TransferSearchStoreSupplierItem> searchStoreSupplierItems)
        {
            using var sqlBulkCopy = new SqlBulkCopy(_connectionString);
            sqlBulkCopy.DestinationTableName = "dbo.TransferSearchStoreSupplier";
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreSupplierItem.TransferSearchStoreId), "TransferSearchStoreID");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreSupplierItem.AccountName), "AccountName");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreSupplierItem.AccountId), "AccountID");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreSupplierItem.System), "System");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreSupplierItem.SupplierName), "SupplierName");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreSupplierItem.SupplierId), "SupplierID");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreSupplierItem.Successful), "Successful");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreSupplierItem.Timeout), "Timeout");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreSupplierItem.SearchDateAndTime), "SearchDateAndTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreSupplierItem.ResultsReturned), "ResultsReturned");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreSupplierItem.PreProcessTime), "PreProcessTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreSupplierItem.SupplierTime), "SupplierTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreSupplierItem.PostProcessTime), "PostProcessTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(TransferSearchStoreSupplierItem.TotalTime), "TotalTime");

            var table = new DataTable();
            table.Columns.AddRange(
                new DataColumn[]
                {
                    new("TransferSearchStoreID", typeof(Guid)),
                    new("AccountName"),
                    new("AccountID"),
                    new("System"),
                    new("SupplierName"),
                    new("SupplierId"),
                    new("Successful"),
                    new("Timeout"),
                    new("SearchDateAndTime"),
                    new("ResultsReturned"),
                    new("PreProcessTime"),
                    new("SupplierTime"),
                    new("PostProcessTime"),
                    new("TotalTime")
                });

            foreach (var item in searchStoreSupplierItems)
            {
                table.Rows.Add(
                    item.TransferSearchStoreId,
                    item.AccountName,
                    item.AccountId,
                    item.System,
                    item.SupplierName,
                    item.SupplierId,
                    item.Successful,
                    item.Timeout,
                    item.SearchDateAndTime,
                    item.ResultsReturned,
                    item.PreProcessTime,
                    item.SupplierTime,
                    item.PostProcessTime,
                    item.TotalTime);
            }

            await sqlBulkCopy.WriteToServerAsync(table);
        }
    }
}
