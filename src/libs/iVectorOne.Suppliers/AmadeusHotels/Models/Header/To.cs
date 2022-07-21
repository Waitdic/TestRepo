namespace iVectorOne.Suppliers.AmadeusHotels.Models.Header
{
    using System.Xml;
    using System.Xml.Serialization;
    using Soap;

    public class To
    {
        [XmlText]
        public string Message { get; set; } = string.Empty;

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns { get; set; }

        public To()
        {
            Xmlns = new XmlSerializerNamespaces(new XmlQualifiedName[]
            {
                new("add", SoapNamespaces.Add)
            });
        }
    }
}
