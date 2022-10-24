namespace iVectorOne.Suppliers.MTS.Models.Common
{
    using System.Xml.Serialization;

    public class UniqueID
    {
        [XmlAttribute]
        public string Type { get; set; } = string.Empty;

        [XmlAttribute]
        public string ID { get; set; } = string.Empty;

        [XmlAttribute]
        public string ID_Context { get; set; } = string.Empty;
    }
}
