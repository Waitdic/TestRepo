namespace iVectorOne.Suppliers.Models.WelcomeBeds
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class ResGuest
    {
        public ResGuest() { }

        [XmlAttribute("Age")]
        public string Age { get; set; } = string.Empty;

        [XmlArray("Profiles")]
        [XmlArrayItem("ProfileInfo")]
        public List<ProfileInfo> Profiles { get; set; } = new();
    }

    public class ProfileInfo
    {
        public ProfileInfo() { }

        [XmlElement("Profile")]
        public Profile Profile { get; set; } = new();
    }


    public class Profile
    {
        public Profile() { }

        [XmlElement("Customer")]
        public Customer Customer { get; set; } = new();
    }

    public class Customer
    {
        public Customer() { }

        [XmlElement("PersonName")]
        public PersonName PersonName { get; set; } = new();

        [XmlElement("CustomerDocument")]
        public CustomerDocument customerDocument { get; set; } = new();

        [XmlElement("TPA_Extensions")]
        public TpaExtensions TpaExtensions { get; set; } = new();
    }

    public class CustomerDocument
    {
        public CustomerDocument() { }
    }
}
