namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Header
{
    using System.Xml;
    using System.Xml.Serialization;
    using Soap;

    public class TransactionFlowLink
    {
        public Consumer Consumer { get; set; } = new();

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns { get; set; }

        public TransactionFlowLink()
        {
            Xmlns = new XmlSerializerNamespaces(new XmlQualifiedName[]
            {
                new("link", SoapNamespaces.Link)
            });
        }
    }
}
