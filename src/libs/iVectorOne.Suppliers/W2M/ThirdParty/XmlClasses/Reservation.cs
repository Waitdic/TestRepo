using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Reservation")]
    public class Reservation
    {
        [XmlElement(ElementName = "Holder")]
        public Holder Holder { get; set; }
        [XmlElement(ElementName = "Paxes")]
        public Paxes Paxes { get; set; }
        [XmlElement(ElementName = "Comments")]
        public Comments Comments { get; set; }
        [XmlElement(ElementName = "AgenciesData")]
        public AgenciesData AgenciesData { get; set; }
        [XmlElement(ElementName = "Items")]
        public Items Items { get; set; }
        [XmlAttribute(AttributeName = "Locator")]
        public string Locator { get; set; }
        [XmlAttribute(AttributeName = "Status")]
        public string Status { get; set; }
        [XmlAttribute(AttributeName = "Language")]
        public string Language { get; set; }
    }
}
