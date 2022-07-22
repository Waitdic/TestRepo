namespace iVectorOne.CSSuppliers.Models.Altura
{
    using System.Xml.Serialization;

    public class Guest
    {
        public Guest() { }

        [XmlElement("Tittle")]
        public string Tittle { get; set; } = string.Empty;

        [XmlElement("FirstName")]
        public string FirstName { get; set; } = string.Empty;

        [XmlElement("LastName")]
        public string LastName { get; set; } = string.Empty;

        [XmlElement("Email")]
        public string Email { get; set; } = string.Empty;

        [XmlElement("Phone")]
        public string Phone { get; set; } = string.Empty;
    }
}
