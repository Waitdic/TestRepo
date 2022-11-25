namespace iVectorOne.Suppliers.Polaris.Models
{
    using System.Collections.Generic;

    public class Hotel
    {
        public string CheckIn { get; set; } = string.Empty;
        public string CheckOut { get; set; } = string.Empty;
        public string HotelCode { get; set; } = string.Empty;
        public HotelInfo Info { get; set; } = new();
        public string Name { get; set; } = string.Empty;
        public List<RoomRate> RoomRates { get; set; } = new();
    }

}
