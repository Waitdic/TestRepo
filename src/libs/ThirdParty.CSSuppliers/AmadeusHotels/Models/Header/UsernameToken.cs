namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Header
{
    using System.Xml;
    using System.Xml.Serialization;
    using Soap;

    public class UsernameToken
    {
        [XmlAttribute("Id", Namespace = SoapNamespaces.Oas1)]
        public string Id { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public Nonce Nonce { get; set; } = new();

        public Password Password { get; set; } = new();

        [XmlElement(Namespace = SoapNamespaces.Oas1)]
        public string Created { get; set; } = string.Empty;

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns { get; set; }

        public UsernameToken()
        {
            Xmlns = new XmlSerializerNamespaces(new XmlQualifiedName[]
            {
                new("oas1", SoapNamespaces.Oas1)
            });
        }
    }
}
