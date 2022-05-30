namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    using System.Xml.Serialization;

    public class Address
    {
        public Address() { }

        [XmlElement("AddressLine")]
        public string AddressLine { get; set; } = string.Empty;

        [XmlElement("PostalCode")]
        public string PostalCode { get; set; } = string.Empty;

        [XmlElement("CityName")]
        public string CityName { get; set; } = string.Empty;

        [XmlElement("StateProv")]
        public string StateProv { get; set; } = string.Empty;

        [XmlElement("CountryName")]
        public string CountryName { get; set; } = string.Empty;
    }
}
