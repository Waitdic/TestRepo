using System.Collections.Generic;

namespace iVectorOne.Suppliers.GoGlobal.Models
{
    public class Hotel
    {
        public string HotelCode { get; set; } = string.Empty;
        public string HotelName { get; set; } = string.Empty;
        public List<Offer> Offers { get; set; } = new();
    }
}