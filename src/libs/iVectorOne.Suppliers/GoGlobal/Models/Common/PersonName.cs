namespace iVectorOne.Suppliers.GoGlobal.Models
{
    using System.Xml.Serialization;

    public class PersonName
    {
        [XmlAttribute("PersonID")]
        public string PersonID { get; set; } = string.Empty;

        [XmlAttribute("Title")]
        public string Title { get; set; } = string.Empty;

        [XmlAttribute("FirstName")]
        public string FirstName { get; set; } = string.Empty;

        [XmlAttribute("LastName")]
        public string LastName { get; set; } = string.Empty;
    }
}
