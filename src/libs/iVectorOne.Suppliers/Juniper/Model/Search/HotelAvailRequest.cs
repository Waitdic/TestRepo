namespace iVectorOne.CSSuppliers.Juniper.Model
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Serialization;


    // OTA_HotelAvailService
    public class OTA_HotelAvailService
    {
        [XmlElement("OTA_HotelAvailRQ")]
        public HotelAvailRequest HotelAvailRequest { get; set; } = new();
    }

    public class HotelAvailRequest
    {
        [XmlAttribute("PrimaryLangID")]
        public string PrimaryLangId { get; set; } = string.Empty;

        [XmlElement("POS")]
        public Pos Pos { get; set; } = new();

        [XmlArray("AvailRequestSegments")]
        [XmlArrayItem("AvailRequestSegment")]
        public List<AvailRequestSegment> AvailRequestSegmets { get; set; } = new();
    }
}