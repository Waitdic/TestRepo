namespace iVectorOne.Suppliers.TBOHolidays.Models.Common
{
    using System;

    public class HotelResult
    {
        public string HotelCode { get; set; } = string.Empty;

        public string Currency { get; set; } = string.Empty;

        public HotelRoom[] Rooms { get; set; } = Array.Empty<HotelRoom>();



        //public HotelInfo HotelInfo { get; set; } = new();

        //public OptionsForBooking OptionsForBooking { get; set; } = new();

        //public HotelRoom[] HotelRooms { get; set; } = Array.Empty<HotelRoom>();

        //public int ResultIndex { get; set; }
    }
}
