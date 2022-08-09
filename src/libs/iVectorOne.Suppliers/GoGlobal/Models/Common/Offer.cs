using System.Collections.Generic;

namespace iVectorOne.Suppliers.GoGlobal.Models
{
    public class Offer
    {
        public string HotelSearchCode { get; set; } = string.Empty;
        public string CxlDeadLine { get; set; } = string.Empty;
        public bool NonRef { get; set; }
        public List<string> Rooms { get; set; } = new();
        public string RoomBasis { get; set; } = string.Empty;
        public int Availability { get; set; }
        public string TotalPrice { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Remark { get; set; } = string.Empty;
    }
}