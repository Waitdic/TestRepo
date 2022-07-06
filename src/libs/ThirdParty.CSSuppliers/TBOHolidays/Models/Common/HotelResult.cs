namespace ThirdParty.CSSuppliers.TBOHolidays.Models.Common
{
    using System;

    public class HotelResult
    {
        public HotelInfo HotelInfo { get; set; } = new();

        public OptionsForBooking OptionsForBooking { get; set; } = new();

        public HotelRoom[] HotelRooms { get; set; } = Array.Empty<HotelRoom>();

        public int ResultIndex { get; set; }
    }
}
