namespace ThirdParty.CSSuppliers.Models.WelcomeBeds
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class OtaHotelAvailRq : SoapContent
    {
        public OtaHotelAvailRq() { }

        [XmlAttribute("Version")]
        public string Version { get; set; } = string.Empty;

        [XmlArray(ElementName = "AvailRequestSegments")]
        [XmlArrayItem(ElementName = "AvailRequestSegment")]
        public List<AvailRequestSegment> AvailRequestSegmets { get; set; } = new List<AvailRequestSegment>();
    }
}
