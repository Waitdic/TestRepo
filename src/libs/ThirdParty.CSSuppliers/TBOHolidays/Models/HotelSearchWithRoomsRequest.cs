namespace ThirdParty.CSSuppliers.TBOHolidays.Models
{
    using System.Collections.Generic;

    public class HotelSearchWithRoomsRequest : SoapContent
    {
        public string CheckInDate { get; set; } = string.Empty;
        public string CheckOutDate { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;
        public string CityId { get; set; } = string.Empty;
        public string IsNearBySearchAllowed { get; set; } = string.Empty;
        public int NoOfRooms { get; set; }
        public string GuestNationality { get; set; } = string.Empty;
        public List<RoomGuest> RoomGuests { get; set; } = new();
        public string PreferredCurrencyCode { get; set; } = string.Empty;
        public string ResultCount { get; set; } = string.Empty;
    }
}
