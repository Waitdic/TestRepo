namespace ThirdParty.CSSuppliers.AbreuV2.Models
{
    using System.Xml.Serialization;

    public class Guest
    {
        [XmlAttribute("AgeCode")]
        public string AgeCode { get; set; } = string.Empty;

        [XmlAttribute("Count")]
        public int Count { get; set; }
        public bool ShouldSerializeCount() => Count > 0;

        [XmlAttribute("Age")]
        public string Age { get; set; }
        public bool ShouldSerializeAge() => !string.IsNullOrEmpty(Age);

        [XmlAttribute("LeadGuest")]
        public string LeadGuest { get; set; } = string.Empty;
        public bool ShouldSerializeLeadGuest() => !string.IsNullOrEmpty(LeadGuest);

        [XmlElement("PersonName")]
        public PersonName PersonName { get; set; } = new();
        public bool ShouldSerializePersonName() => !string.IsNullOrEmpty(PersonName.GivenName);
    }
}
