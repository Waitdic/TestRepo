namespace iVectorOne.Suppliers.TBOHolidays.Models
{
    using System;
    using Book;
    using Intuitive.Helpers.Net;

    public static class Helper
    {
        public static string[] Separators = { "~~~" };

        public static string GetAuth(string user, string password)
        {
            return "Basic " + Convert.ToBase64String(
                System.Text.Encoding.ASCII.GetBytes($"{user}:{password}"));
        }
    }

    public class BookingDetailRequest
    {
        public Request BookWebRequest { get; set; }
        public HotelBookRequest BookRequest { get; set; }
        public HotelBookResponse? BookResponse { get; set; }
        public Request? BookDetailWebRequest { get; set; }
        public HotelBookingDetailResponse? BookingDetailResponse { get; set; }
    }
}
