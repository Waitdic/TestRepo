namespace iVectorOne.Suppliers.JonView.Models
{
    using System.Xml.Serialization;

    public class RequestCall
    {
        [XmlAttribute("encodingStyle")]
        public string EncodingStyle { get; set; } = "http://schemas.xmlsoap.org/soap/encoding/";

        [XmlElement("as_type")]
        public string AsType { get; set; } = string.Empty;

        [XmlElement("as_cache")]
        public string AsCache { get; set; } = string.Empty;

        [XmlElement("as_userid")]
        public string User { get; set; } = string.Empty;

        [XmlElement("as_password")]
        public string Password { get; set; } = string.Empty;

        [XmlElement("as_client_loc_seq")]
        public string ClientLocSeq { get; set; } = string.Empty;

        [XmlElement("as_message")]
        public Message Message { get; set; } = new();
    }

    public class Message 
    {
        [XmlText]
        public string Content { get; set; } = string.Empty;
    }
}
