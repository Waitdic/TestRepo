namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    using System.Xml.Serialization;

    public class Customer
    {
        public Customer() { }

        [XmlElement("PersonName")]
        public PersonName PersonName { get; set; } = new();

        [XmlElement("Email")]
        public string Email { get; set; } = string.Empty;

        [XmlElement("Address")]
        public Address Address { get; set; } = new();

    }
}
