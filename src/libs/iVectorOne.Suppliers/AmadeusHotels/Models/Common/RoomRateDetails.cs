namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class RoomRateDetails
    {
        [XmlElement("marker")]
        public Marker Marker { get; set; } = new();

        [XmlElement("markerOfExtra")]
        public Marker MarkerOfExtra { get; set; } = new();

        [XmlElement("hotelProductReference")]
        public HotelProductReference HotelProductReference { get; set; } = new();
    }
}
