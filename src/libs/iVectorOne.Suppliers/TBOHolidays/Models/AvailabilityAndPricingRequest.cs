﻿namespace iVectorOne.Suppliers.TBOHolidays.Models
{
    using Common;

    public class AvailabilityAndPricingRequest : SoapContent
    {
        public int ResultIndex { get; set; }

        public string HotelCode { get; set; } = string.Empty;

        public string SessionId { get; set; } = string.Empty;

        public OptionsForBooking OptionsForBooking { get; set; } = new();
    }
}
