namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class GuestCount
    {
        [XmlAttribute("Count")]
        public int Count { get; set; }

        [XmlAttribute("Age")]
        public int Age { get; set; }
        public bool ShouldSerializeAge() => Age > 0;

        [XmlAttribute("AgeQualifyingCode")]
        public string AgeQualifyingCode { get; set; } = string.Empty;
        public bool ShouldSerializeAgeQualifyingCode() => !string.IsNullOrEmpty(AgeQualifyingCode);
    }
}
