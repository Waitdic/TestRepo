namespace iVectorOne.Models.SearchStore
{
    using System;
    public class TransferSearchStoreItem
    {
        public Guid TransferSearchStoreId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public int AccountId { get; set; }
        public string System { get; set; } = string.Empty;
        public bool Successful { get; set; }
        public DateTime SearchDateAndTime { get; set; }
        //public int PropertiesRequested { get; set; }
        public int ResultsReturned { get; set; }
        public int PreProcessTime { get; set; }
        public int MaxSupplierTime { get; set; }
        public int PostProcessTime { get; set; }
        public int TotalTime { get; set; }
    }
}
