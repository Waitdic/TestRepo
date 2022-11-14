namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Address
    {
        [XmlElement("AddressLine")]
        public List<string> AddressLines { get; set; } = new();
        public string CityName { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public CountryName CountryName { get; set; } = new();
    }


}
