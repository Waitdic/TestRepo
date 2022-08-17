namespace iVectorOne.Suppliers.Serhs.Models.Common
{
    using System.Xml.Serialization;
    public class Customer
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("surname")]
        public string? Surname { get; set; }

        [XmlElement("address")]
        public string? Address { get; set; }

        [XmlElement("city")]
        public City City { get; set; } = new();

        [XmlElement("region")]
        public Region Region { get; set; } = new();

        [XmlElement("document")]
        public Document Document { get; set; } = new();

        [XmlElement("contactInfo")]
        public ContactInfo ContactInfo { get; set; } = new();
    }
}