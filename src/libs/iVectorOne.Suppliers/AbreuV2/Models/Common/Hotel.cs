namespace iVectorOne.Suppliers.AbreuV2.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Hotel
    {
        [XmlElement("Info")]
        public HotelInfo Info { get; set; } = new();

        [XmlElement("BestPrice")]
        public BestPrice BestPrice { get; set; } = new();

        [XmlArray("Rooms")]
        [XmlArrayItem("Room")]
        public List<Room> Rooms { get; set; } = new();
    }
}
