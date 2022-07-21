using System.Collections.Generic;
using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "HotelResult")]
    public class HotelResult
    {
        [XmlArray(ElementName = "HotelOptions")]
        [XmlArrayItem(ElementName = "HotelOption")]
        public List<HotelOption> HotelOptions { get; set; }

        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }

        [XmlAttribute(AttributeName = "JPCode")]
        public string JPCode { get; set; }

        [XmlAttribute(AttributeName = "JPDCode")]
        public string JPDCode { get; set; }

        [XmlAttribute(AttributeName = "BestDeal")]
        public string BestDeal { get; set; }

        [XmlAttribute(AttributeName = "DestinationZone")]
        public string DestinationZone { get; set; }
    }

    [XmlRoot(ElementName = "HotelResult")]
    public class HotelAvailResponseHotelResult
    {
        [XmlArray(ElementName = "HotelOptions")]
        [XmlArrayItem(ElementName = "HotelOption")]
        public List<HotelOption> HotelOptions { get; set; }

        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }

        [XmlAttribute(AttributeName = "JPCode")]
        public string JPCode { get; set; }

        [XmlAttribute(AttributeName = "JPDCode")]
        public string JPDCode { get; set; }

        [XmlAttribute(AttributeName = "BestDeal")]
        public string BestDeal { get; set; }

        [XmlAttribute(AttributeName = "DestinationZone")]
        public string DestinationZone { get; set; }
    }
}
