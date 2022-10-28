namespace iVectorOne_Admin_Api.Data.Models.Dashboard
{
    public class DashboardSummary
    {
        public string Row { get; set; } = string.Empty; 

        public int BookingTotal { get; set; }

        public decimal BookingValue { get; set; }

        public int PrebookTotal { get; set; }

        public int PrebookSuccess { get; set; }

        public int SearchTotal { get; set; }

        public int SearchSuccessful { get; set; }

        public int AverageSearchTime { get; set; }



    }
}
