namespace iVectorOne.Suppliers.AmadeusHotels.Models
{
    using System.Xml;
    using System.Xml.Serialization;
    using Soap;

    public class SecuritySignOut : SoapContent
    {
        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns { get; set; }

        public SecuritySignOut()
        {
            Xmlns = new XmlSerializerNamespaces(new XmlQualifiedName[]
            {
                new(null, "http://xml.amadeus.com/VLSSOQ_04_1_1A")
            });
        }
    }
}