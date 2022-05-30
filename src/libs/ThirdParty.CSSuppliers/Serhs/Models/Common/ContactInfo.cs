namespace ThirdParty.CSSuppliers.Serhs.Models.Common
{
    using System.Xml.Serialization;

    public class ContactInfo
    {
        [XmlAttribute("email")]
        public string? Email { get; set; }

        [XmlAttribute("mobilePhone")]
        public string? MobilePhone { get; set; }
    }
}
