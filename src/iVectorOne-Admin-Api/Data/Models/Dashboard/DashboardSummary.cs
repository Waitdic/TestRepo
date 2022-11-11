namespace iVectorOne_Admin_Api.Data.Models.Dashboard
{
    public class DashboardSummary
    {
        public string Row { get; set; } = string.Empty;

        public int BookingTotal { get; set; }

        public decimal BookingValue { get; set; }

        public int PrebookTotal { get; set; }

        public int PrebookSuccess { get; set; }

        public float PrebookSuccessfulPrecent => PrebookTotal == 0.0 ? 0 : (PrebookSuccess / (float)PrebookTotal) * 100;

        public int SearchTotal { get; set; }

        public int SearchSuccessful { get; set; }

        public float SearchSuccessfulPrecent => SearchTotal == 0.0 ? 0 : (SearchSuccessful / (float)SearchTotal) * 100;

        public int AverageSearchTime { get; set; }

        public float S2B => BookingTotal == 0 ? 0 : (SearchTotal / (float)BookingTotal);
    }
}
