namespace iVectorOne.Suppliers.AmadeusHotels.Models.Header
{
    using System.Xml;
    using System.Xml.Serialization;
    using Soap;

    public class Action
    {
        [XmlText]
        public string Message { get; set; } = string.Empty;

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns { get; set; }

        public Action()
        {
            Xmlns = new XmlSerializerNamespaces(new XmlQualifiedName[]
            {
                new("add", SoapNamespaces.Add)
            });
        }
    }
}
