namespace iVectorOne.CSSuppliers.AmadeusHotels.Models.Header
{
    using System.Xml;
    using System.Xml.Serialization;
    using Soap;

    public class MessageID
    {
        [XmlText]
        public string Message { get; set; } = string.Empty;

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns { get; set; }

        public MessageID()
        {
            Xmlns = new XmlSerializerNamespaces(new XmlQualifiedName[]
            {
                new("add", SoapNamespaces.Add)
            });
        }
    }
}
