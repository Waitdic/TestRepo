namespace ThirdParty.CSSuppliers.Models.WelcomeBeds
{
    using System.Xml.Serialization;

    public class PersonName
    {
        public PersonName() { }
        [XmlAttribute("GivenName")]
        public string GivenName { get; set; } = string.Empty;
        [XmlAttribute("Surname")]
        public string Surname { get; set; } = string.Empty;
    }
}
