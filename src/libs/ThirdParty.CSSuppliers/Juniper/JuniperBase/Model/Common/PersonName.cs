namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    using System.Xml.Serialization;

    public class PersonName
    {
        public PersonName() { }
        [XmlElement("GivenName")]
        public string GivenName { get; set; } = string.Empty;

        [XmlElement("Surname")]
        public string Surname { get; set; } = string.Empty;
    }
}
