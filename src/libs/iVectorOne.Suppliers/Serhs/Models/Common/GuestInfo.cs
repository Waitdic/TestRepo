namespace iVectorOne.Suppliers.Serhs.Models.Common
{
    using System.Xml.Serialization;

    public class GuestInfo
    {
        [XmlAttribute("documentType")]
        public string? DocumentType { get; set; }

        [XmlAttribute("document")]
        public string? Document { get; set; }

        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("surname")]
        public string? Surname { get; set; }

        [XmlAttribute("age")]
        public int Age { get; set; }
    }
}