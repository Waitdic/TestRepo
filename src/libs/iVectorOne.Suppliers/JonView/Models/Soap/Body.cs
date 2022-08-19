namespace iVectorOne.Suppliers.JonView.Models
{
    using System.Xml.Serialization;

    public class Body
    {
        [XmlElement(Namespace = XmlNamespaces.Soap, ElementName = "uf_process_request_call")]
        public RequestCall RequestCall { get; set; } = new();
    }
}
