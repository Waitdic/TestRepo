namespace iVectorOne.Suppliers.JonView.Models
{
    using System.Xml;
    using System.Xml.Serialization;

    [XmlRoot(Namespace = XmlNamespaces.SoapEnv, ElementName = "Envelope")]
    public class Envelope
    {
        public Envelope()
        {
            Xmlns = new(new XmlQualifiedName[]
            {
                new("soapenv", XmlNamespaces.SoapEnv),
                new("soap", XmlNamespaces.Soap)
            });
        }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns { get; set; }

        public Header Header { get; set; } = new();
        public Body Body { get; set; } = new();
    }

    public static class XmlNamespaces 
    {
        public const string Soap = "http://SoapJonviewHostImpl.test.jonview.com";
        public const string SoapEnv = "http://schemas.xmlsoap.org/soap/envelope/";
    }

    public interface IMessageRq { }
    public interface IMessageRs { }
}
