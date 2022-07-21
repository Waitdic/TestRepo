namespace ThirdParty.CSSuppliers.AmadeusHotels.Models
{
    using System.Xml.Serialization;
    using Common;
    using Soap;

    public class HotelSellReply : SoapContent
    {
        [XmlElement("roomStayData")]
        public RoomStayData RoomStayData { get; set; } = new();
    }
}
