using System;

namespace iVectorOne.Models.SearchStore
{
    public class SearchStoreItem
    {
        public Guid SearchStoreId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public int AccountId { get; set; }
        public string System { get; set; } = string.Empty;
        public bool Successful { get; set; }
        public DateTime SearchDateAndTime { get; set; }
        public int PropertiesRequested { get; set; }
        public int PropertiesReturned { get; set; }
        public int PreProcessTime { get; set; }
        public int MaxSupplierTime { get; set; }
        public int MaxDedupeTime { get; set; }
        public int PostProcessTime { get; set; }
        public int TotalTime { get; set; }
    }
}
