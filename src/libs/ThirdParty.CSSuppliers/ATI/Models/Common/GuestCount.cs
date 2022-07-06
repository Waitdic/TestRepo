namespace ThirdParty.CSSuppliers.ATI.Models.Common
{
    using System.Xml.Serialization;

    public class GuestCount
    {
        [XmlAttribute("AgeQualifyingCode")]
        public string AgeQualifyingCode { get; set; } = string.Empty;

        [XmlAttribute("Age")]
        public int Age { get; set; }
        public bool ShouldSerializeAge() => Age > 0;

        [XmlAttribute("Count")]
        public int Count { get; set; }

        [XmlAttribute("ResGuestRPH")]
        public int ResGuestRPH { get; set; }
        public bool ShouldSerializeResGuestRPH() => ResGuestRPH > 0;
    }
}
