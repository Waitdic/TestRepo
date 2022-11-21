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
}
