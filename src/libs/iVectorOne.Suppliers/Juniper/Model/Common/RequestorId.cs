namespace iVectorOne.CSSuppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class RequestorId
    {
        public RequestorId() { }

        [XmlAttribute("Type")]
        public string TypeCode { get; set; } = "1";

        [XmlAttribute("MessagePassword")]
        public string MessagePassword { get; set; } = string.Empty;
    }
}