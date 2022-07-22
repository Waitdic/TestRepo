using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Pax")]
    public class Pax
    {
        [XmlAttribute(AttributeName = "IdPax")]
        public int IdPax { get; set; }
        [XmlElement(ElementName = "Age")]
        public string Age { get; set; }
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Surname")]
        public string Surname { get; set; }

        [XmlElement(ElementName = "Document")]
        public Document Document { get; set; }
        [XmlElement(ElementName = "PhoneNumbers")]
        public PhoneNumbers PhoneNumbers { get; set; }
        [XmlElement(ElementName = "Email")]
        public string Email { get; set; }
        [XmlElement(ElementName = "Address")]
        public string Address { get; set; }
        [XmlElement(ElementName = "City")]
        public string City { get; set; }
        [XmlElement(ElementName = "Country")]
        public string Country { get; set; }
        [XmlElement(ElementName = "PostalCode")]
        public string PostalCode { get; set; }
        [XmlElement(ElementName = "Nationality")]
        public string Nationality { get; set; }
    }
}
