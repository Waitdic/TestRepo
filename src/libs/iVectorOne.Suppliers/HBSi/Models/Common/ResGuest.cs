namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class ResGuest
    {
        [XmlAttribute("ResGuestRPH")]
        public int ResGuestRPH { get; set; }

        [XmlAttribute("Age")]
        public string Age { get; set; } = string.Empty;
        public bool ShouldSerializeAge() => !string.IsNullOrEmpty(Age);

        [XmlAttribute("AgeQualifyingCode")]
        public string AgeQualifyingCode { get; set; } = string.Empty;
        public bool ShouldSerializeAgeQualifyingCode() => !string.IsNullOrEmpty(AgeQualifyingCode);

        [XmlArray("Profiles")]
        [XmlArrayItem("ProfileInfo")]
        public List<ProfileInfo> Profiles { get; set; } = new();
    }

    public class ProfileInfo
    {
        [XmlElement("Profile")]
        public Profile Profile { get; set; } = new();
    }


    public class Profile
    {
        [XmlAttribute("ProfileType")]
        public int ProfileType { get; set; }
        public bool ShouldSerializeProfileType() => ProfileType > 0;

        [XmlElement("Customer")]
        public Customer Customer { get; set; } = new();
    }

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

    public class Address
    {
        [XmlElement("AddressLine")]
        public List<string> AddressLines { get; set; } = new();
        public string CityName { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public CountryName CountryName { get; set; } = new();
    }

    public class CountryName
    {
        [XmlAttribute("Code")]
        public string Code { get; set; } = string.Empty;
    }

    public class Telephone
    {
        [XmlAttribute("PhoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;
    }


}
