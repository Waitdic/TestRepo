namespace iVectorOne_Admin_Api.Data.Models.Dashboard
{
    public class BookingsByHour
    {
        public int Hour { get; set; }

        public int CurrentWeek { get; set; }

        public int PreviousWeek { get; set; }
    }
}
