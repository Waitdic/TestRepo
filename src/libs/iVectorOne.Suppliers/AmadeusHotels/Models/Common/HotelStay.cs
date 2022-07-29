namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class HotelStay
    {
        public BasicPropertyInfo BasicPropertyInfo { get; set; } = new();

        [XmlAttribute("RoomStayRPH")]
        public string RoomStayRph { get; set; } = string.Empty;
    }
}
