namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Header
{
    using System.Xml;
    using System.Xml.Serialization;
    using Soap;
    
    public class SessionRequest : Session
    {
        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns { get; set; }

        public SessionRequest()
        {
            Xmlns = new XmlSerializerNamespaces(new XmlQualifiedName[]
            {
                new("ses", SoapNamespaces.Ses),
            });
        }
    }
}
