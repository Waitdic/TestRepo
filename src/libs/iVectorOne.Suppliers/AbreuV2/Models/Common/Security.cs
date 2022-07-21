using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.AbreuV2.Models
{
    public class Security
    {
        public Security()
        {
            xmlns.Add("wsse", SoapNamespaces.Wsse);
        }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();

        [XmlElement(Namespace = SoapNamespaces.Wsse)]
        public string Username { get; set; } = string.Empty;

        [XmlElement(Namespace = SoapNamespaces.Wsse)]
        public string Password { get; set; } = string.Empty;

        [XmlElement(Namespace = "")]
        public string Context { get; set; } = string.Empty;
    }
}
