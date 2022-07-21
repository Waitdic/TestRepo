namespace iVectorOne.CSSuppliers.AmadeusHotels.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Common;
    using Soap;

    public class OTAHotelAvailRS : SoapContent
    {
        [XmlElement("RoomStays")]
        public RoomStays RoomStays { get; set; } = new();

        [XmlArray("HotelStays")]
        [XmlArrayItem("HotelStay")]
        public List<HotelStay> HotelStays { get; set; } = new();
    }
}