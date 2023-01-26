namespace iVectorOne.Models.SearchStore
{
    using System;
    public class ExtraSearchStoreSupplierItem
    {
        public Guid ExtraSearchStoreId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public int AccountId { get; set; }
        public string System { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public bool Successful { get; set; }
        public bool Timeout { get; set; }
        public DateTime SearchDateAndTime { get; set; }
        //public int PropertiesRequested { get; set; }
        public int ResultsReturned { get; set; }
        public int PreProcessTime { get; set; }
        public int SupplierTime { get; set; }
        public int PostProcessTime { get; set; }
        public int TotalTime { get; set; }
    }
}