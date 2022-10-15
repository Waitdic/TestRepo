namespace iVectorOne.Suppliers.Italcamel.Models.Common
{
    using System.Xml.Serialization;

    public class CancellationPolicy
    {
        [XmlElement("DAYFROM")]
        public int DayFrom { get; set; }
        
        [XmlElement("DAYTO")]
        public int DayTo { get; set; }
        
        [XmlElement("ROOMUID")]
        public string RoomUID { get; set; } = string.Empty;

        [XmlElement("TYPE")]
        public string Type { get; set; } = string.Empty;

        [XmlElement("VALUE")]
        public decimal Value { get; set; }

        [XmlElement("DESCRIPTION")]
        public string Description { get; set; } = string.Empty;
    }
}
