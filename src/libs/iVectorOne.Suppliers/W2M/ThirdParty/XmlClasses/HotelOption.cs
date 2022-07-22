using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "HotelOption")]
    public class HotelOption
    {
        [XmlElement(ElementName = "Board")]
        public Board Board { get; set; }
        [XmlElement(ElementName = "HotelRooms")]
        public HotelRooms HotelRooms { get; set; }
        [XmlElement(ElementName = "Prices")]
        public Prices Prices { get; set; }
        [XmlAttribute(AttributeName = "RatePlanCode")]
        public string RatePlanCode { get; set; }
        [XmlAttribute(AttributeName = "Status")]
        public string Status { get; set; }
        [XmlElement(ElementName = "BookingCode")]
        public BookingCode BookingCode { get; set; }
        [XmlElement(ElementName = "HotelRequiredFields")]
        public HotelRequiredFields HotelRequiredFields { get; set; }
        [XmlElement(ElementName = "CancellationPolicy")]
        public CancellationPolicy CancellationPolicy { get; set; }
        [XmlElement(ElementName = "PriceInformation")]
        public PriceInformation PriceInformation { get; set; }
        [XmlElement(ElementName = "OptionalElements")]
        public OptionalElements OptionalElements { get; set; }
        [XmlElement(ElementName = "AdditionalElements")]
        public AdditionalElements AdditionalElements { get; set; }
        [XmlAttribute(AttributeName = "NonRefundable")]
        public bool NonRefundable { get; set; }
        [XmlAttribute(AttributeName = "PackageContract")]
        public bool PackageContract { get; set; }
    }
}
