namespace iVectorOne_Admin_Api.Data.Models
{
    public partial class DashboardTable
    {
        public int SearchStoreID { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public int AccountID { get; set; }
        public string System { get; set; } = string.Empty;
        public bool Successful { get; set; }
        public string SearchDateAndTime { get; set; } = string.Empty;
        public int PropertiesRequested { get; set; }
        public int PropertiesReturned { get; set; }
        public int PreprocessTime { get; set; }
        public int MaxSupplierTime { get; set; }
        public int MaxDedupeTime { get; set; }
        public int PostProcessTime { get; set; }
        public int TotalTime { get; set; }

        public Tenant? Tenant { get; set; }
        public Account? Account { get; set; }
    }
}