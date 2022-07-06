namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Header
{
    using System.Xml.Serialization;
    using Soap;
    using System.Xml;

    public class SoapHeader
    {
        [XmlElement(Namespace = SoapNamespaces.Ses)]
        public SessionRequest? Session { get; set; }
        public bool ShouldSerializeSession() => Session != null;

        [XmlElement(Namespace = SoapNamespaces.Add)]
        public MessageID MessageID { get; set; } = new();

        [XmlElement(Namespace = SoapNamespaces.Add)]
        public Action Action { get; set; } = new();

        [XmlElement(Namespace = SoapNamespaces.Add)]
        public To To { get; set; } = new();

        [XmlElement(Namespace = SoapNamespaces.Link)]
        public TransactionFlowLink TransactionFlowLink { get; set; } = new();

        [XmlElement(Namespace = SoapNamespaces.Oas)]
        public Security? Security { get; set; }
        public bool ShouldSerializeSecurity() => Security != null;

        [XmlElement("AMA_SecurityHostedUser", Namespace = "http://xml.amadeus.com/2010/06/Security_v1")]
        public AmaSecurityHostedUser AmaSecurityHostedUser { get; set; } = new();

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns { get; set; }

        public SoapHeader()
        {
            Xmlns = new XmlSerializerNamespaces(new XmlQualifiedName[]
            {
                new("soap", SoapNamespaces.Soap),
                new("awsse", SoapNamespaces.Ses),
            });
        }
    }
}
