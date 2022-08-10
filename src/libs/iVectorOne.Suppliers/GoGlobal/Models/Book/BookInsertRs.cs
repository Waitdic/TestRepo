namespace iVectorOne.Suppliers.GoGlobal.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class BookInsertRs : Main
    {
        public string GoBookingCode { get; set; } = string.Empty;
        public string GoReference { get; set; } = string.Empty;
        public string ClientBookingCode { get; set; } = string.Empty;
        public string BookingStatus { get; set; } = string.Empty;
        public string TotalPrice { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string HotelName { get; set; } = string.Empty;
        public string HotelSearchCode { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public string RoomBasis { get; set; } = string.Empty;
        public string ArrivalDate { get; set; } = string.Empty;
        public string CancellationDeadline { get; set; } = string.Empty;
        public string Nights { get; set; } = string.Empty;
        public string NoAlternativeHotel { get; set; } = string.Empty;
        public Leader Leader { get; set; } = new();

        [XmlArray("Rooms")]
        [XmlArrayItem("RoomType")]
        public List<RoomType> Rooms { get; set; } = new();
        public Preferences Preferences { get; set; } = new();
        public string Remark { get; set; } = string.Empty;
    }
}
