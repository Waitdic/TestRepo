namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class Customer
    {
        [XmlElement("PersonName")]
        public PersonName PersonName { get; set; } = new();

        [XmlElement("Telephone")]
        public Telephone Telephone { get; set; } = new();
        public bool ShouldSerializeTelephone() => !string.IsNullOrEmpty(Telephone?.PhoneNumber ?? "");

        [XmlElement("Email")]
        public string Email { get; set; } = string.Empty;
        public bool ShouldSerializeEmail() => !string.IsNullOrEmpty(Email);

        [XmlElement("Address")]
        public Address Address { get; set; } = new();
        public bool ShouldSerializeAddress() => !string.IsNullOrEmpty(Address?.CityName ?? "");
    }


}
