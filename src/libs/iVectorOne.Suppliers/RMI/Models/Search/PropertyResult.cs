namespace iVectorOne.Suppliers.RMI.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class PropertyResult
    {
        [XmlElement("PropertyID")]
        public string PropertyId { get; set; } = string.Empty;

        [XmlArray("RoomTypes")]
        [XmlArrayItem("RoomType")]
        public List<RoomType> RoomTypes { get; set; } = new();

        [XmlArray("Errata")]
        [XmlArrayItem("Erratum")]
        public List<Erratum> Errata { get; set; } = new();
    }
}
