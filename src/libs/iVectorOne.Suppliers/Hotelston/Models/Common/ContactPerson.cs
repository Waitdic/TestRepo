namespace iVectorOne.CSSuppliers.Hotelston.Models.Common
{
    using System.Xml.Serialization;

    public class ContactPerson
    {
        [XmlAttribute("title")]
        public string Title { get; set; } = string.Empty;

        [XmlAttribute("firstname")]
        public string Firstname { get; set; } = string.Empty;

        [XmlAttribute("lastname")]
        public string Lastname { get; set; } = string.Empty;

        [XmlAttribute("email")]
        public string Email { get; set; } = string.Empty;

        [XmlAttribute("phone")]
        public string Phone { get; set; } = string.Empty;
    }
}