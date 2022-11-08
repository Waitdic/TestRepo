namespace iVectorOne_Admin_Api.Data.Models.Dashboard
{
    public class SupplierSummary
    {
        public int SupplierId { get; set; }

        public string SupplierName { get; set; } = string.Empty;

        public int BookTotal { get; set; }

        public int BookSuccess { get; set; }

        public float BookSuccessfulPrecent => BookTotal == 0 ? 0 : (BookSuccess / (float)BookTotal) * 100;

        public int PrebookTotal { get; set; }

        public int PrebookSuccess { get; set; }

        public float PrebookSuccessfulPrecent => PrebookTotal == 0 ? 0 : (PrebookSuccess / (float)PrebookTotal) * 100;

        public int SearchTotal { get; set; }

        public int SearchSuccess { get; set; }

        public float SearchSuccessfulPrecent => SearchTotal == 0 ? 0 : (SearchSuccess / (float)SearchTotal) * 100;

        public int AverageSearchTime { get; set; }

        public float S2B => BookTotal == 0 ? 0 : (SearchTotal / (float)BookTotal);
    }
}
