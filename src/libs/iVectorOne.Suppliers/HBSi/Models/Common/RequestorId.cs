namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class RequestorId
    {
        public RequestorId() { }

        [XmlAttribute("Type")]
        public string TypeCode { get; set; } = string.Empty;

        [XmlAttribute("ID")]
        public string ID { get; set; } = string.Empty;

    }
}
