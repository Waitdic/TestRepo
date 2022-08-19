namespace iVectorOne.Suppliers.JonView.Models
{
    using System.Xml.Serialization;

    [XmlRoot("Envelope")]
    public class RsEnvelope 
    {
        public RsBody Body { get; set; } = new();
    }

    public class RsBody
    {
        [XmlElement("uf_process_request_callResponse")]
        public CallResponse CallResponse { get; set; } = new();
    }

    public class CallResponse
    {
        [XmlElement("return")]
        public string Return { get; set; } = string.Empty;
    }
}
