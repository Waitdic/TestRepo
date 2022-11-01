namespace iVectorOne_Admin_Api.Data.Models.Dashboard
{
    public class SupplierSummary
    {
        public int SupplierId { get; set; }

        public string SupplierName { get; set; } = string.Empty;

        public int BookTotal { get; set; }

        public int BookSuccess { get; set; }

        public int PrebookTotal { get; set; }

        public int PrebookSuccess { get; set; }

        public int SearchTotal { get; set; }

        public int SearchSuccess { get; set; }

        public int AverageSearchTime { get; set; }



    }
}
