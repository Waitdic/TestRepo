namespace iVectorOne.Suppliers.MTS.Models.Common
{
    using System.Xml.Serialization;
    public class GuestCount
    {
        [XmlAttribute]
        public string AgeQualifyingCode { get; set; } = string.Empty;
        public bool ShouldSerializeAgeQualifyingCode() => !string.IsNullOrEmpty(AgeQualifyingCode);

        [XmlAttribute]
        public int Count { get; set; }
        public bool ShouldSerializeCount() => Count != 0;

        [XmlAttribute]
        public int Age { get; set; }
        public bool ShouldSerializeAge() => Age != 0;
    }
}
