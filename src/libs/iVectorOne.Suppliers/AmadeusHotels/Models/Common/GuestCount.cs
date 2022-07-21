namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class GuestCount
    {
        [XmlAttribute("AgeQualifyingCode")]
        public string AgeQualifyingCode { get; set; } = string.Empty;

        [XmlAttribute("Count")]
        public int Count { get; set; }
    }
}
