namespace iVectorOne.Suppliers.TBOHolidays.Models.Common
{
    using System;

    public class HotelResult
    {
        public string HotelCode { get; set; } = string.Empty;

        public string Currency { get; set; } = string.Empty;

        public HotelRoom[] Rooms { get; set; } = Array.Empty<HotelRoom>();

        public string[] RateConditions { get; set; } = Array.Empty<string>();
    }
}
