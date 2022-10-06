namespace iVectorOne.Suppliers.Italcamel.Models.Common
{
    using System.Xml.Serialization;

    public class CancellationPolicy
    {
        [XmlElement("ROOMUID")]
        public string RoomUID { get; set; } = string.Empty;

        [XmlElement("RELEASEDATE")]
        public string ReleaseDate { get; set; } = string.Empty;

        [XmlElement("TYPE")]
        public string Type { get; set; } = string.Empty;

        [XmlElement("VALUE")]
        public decimal Value { get; set; }

        [XmlElement("DESCRIPTION")]
        public string Description { get; set; } = string.Empty;
    }
}
