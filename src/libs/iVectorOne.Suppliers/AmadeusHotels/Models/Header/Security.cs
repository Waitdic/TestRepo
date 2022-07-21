namespace iVectorOne.CSSuppliers.AmadeusHotels.Models.Header
{
    using System.Xml;
    using System.Xml.Serialization;
    using Soap;

    public class Security
    {
        public UsernameToken UsernameToken { get; set; } = new();

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns { get; set; }

        public Security()
        {
            Xmlns = new XmlSerializerNamespaces(new XmlQualifiedName[]
            {
                new("oas", SoapNamespaces.Oas)
            });
        }
    }
}
