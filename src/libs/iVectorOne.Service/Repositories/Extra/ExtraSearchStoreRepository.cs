namespace iVectorOne.Repositories
{
    using iVectorOne.Models.SearchStore;
    using Microsoft.Data.SqlClient;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Threading.Tasks;
    public class ExtraSearchStoreRepository : IExtraSearchStoreRepository
    {
        private readonly string _connectionString;
        public ExtraSearchStoreRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public async Task BulkInsertAsync(IEnumerable<ExtraSearchStoreItem> searchStoreItems)
        {
            using var sqlBulkCopy = new SqlBulkCopy(_connectionString);
            sqlBulkCopy.DestinationTableName = "dbo.ExtraSearchStoreApi";
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreItem.ExtraSearchStoreId), "ExtraSearchStoreID");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreItem.AccountName), "AccountName");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreItem.AccountId), "AccountID");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreItem.System), "System");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreItem.Successful), "Successful");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreItem.SearchDateAndTime), "SearchDateAndTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreItem.ResultsReturned), "ResultsReturned");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreItem.PreProcessTime), "PreProcessTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreItem.MaxSupplierTime), "MaxSupplierTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreItem.PostProcessTime), "PostProcessTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreItem.TotalTime), "TotalTime");

            var table = new DataTable();
            table.Columns.AddRange(
                new DataColumn[]{
                new("ExtraSearchStoreID", typeof(Guid)),
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
                    item.ExtraSearchStoreId,
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

        public async Task BulkInsertAsync(IEnumerable<ExtraSearchStoreSupplierItem> searchStoreSupplierItems)
        {
            using var sqlBulkCopy = new SqlBulkCopy(_connectionString);
            sqlBulkCopy.DestinationTableName = "dbo.ExtraSearchStoreSupplier";
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreSupplierItem.ExtraSearchStoreId), "ExtraSearchStoreID");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreSupplierItem.AccountName), "AccountName");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreSupplierItem.AccountId), "AccountID");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreSupplierItem.System), "System");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreSupplierItem.SupplierName), "SupplierName");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreSupplierItem.SupplierId), "SupplierID");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreSupplierItem.Successful), "Successful");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreSupplierItem.Timeout), "Timeout");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreSupplierItem.SearchDateAndTime), "SearchDateAndTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreSupplierItem.ResultsReturned), "ResultsReturned");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreSupplierItem.PreProcessTime), "PreProcessTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreSupplierItem.SupplierTime), "SupplierTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreSupplierItem.PostProcessTime), "PostProcessTime");
            sqlBulkCopy.ColumnMappings.Add(nameof(ExtraSearchStoreSupplierItem.TotalTime), "TotalTime");

            var table = new DataTable();
            table.Columns.AddRange(
                new DataColumn[]
                {
                    new("ExtraSearchStoreID", typeof(Guid)),
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
                    item.ExtraSearchStoreId,
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
