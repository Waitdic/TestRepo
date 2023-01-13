namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class PersonName
    {
        [XmlElement("GivenName")]
        public string GivenName { get; set; } = string.Empty;
        [XmlElement("Surname")]
        public string Surname { get; set; } = string.Empty;
    }
}
