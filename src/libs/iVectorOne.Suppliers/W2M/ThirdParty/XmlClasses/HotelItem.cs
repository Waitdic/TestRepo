using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "HotelItem")]
    public class HotelItem
    {
        [XmlElement(ElementName = "Prices")]
        public Prices Prices { get; set; }

        [XmlElement(ElementName = "CancellationPolicy")]
        public CancellationPolicy CancellationPolicy { get; set; }

        [XmlElement(ElementName = "Comments")]
        public Comments Comments { get; set; }

        [XmlElement(ElementName = "HotelInfo")]
        public HotelInfo HotelInfo { get; set; }

        [XmlElement(ElementName = "Board")]
        public string Board { get; set; }

        [XmlElement(ElementName = "HotelRooms")]
        public HotelRooms HotelRooms { get; set; }

        [XmlAttribute(AttributeName = "ItemId")]
        public string ItemId { get; set; }

        [XmlAttribute(AttributeName = "Status")]
        public string Status { get; set; }

        [XmlAttribute(AttributeName = "Start")]
        public string Start { get; set; }

        [XmlAttribute(AttributeName = "End")]
        public string End { get; set; }
    }
}
