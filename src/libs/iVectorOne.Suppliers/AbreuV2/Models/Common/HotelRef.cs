namespace iVectorOne.CSSuppliers.AbreuV2.Models
{
    using System.Xml.Serialization;

    public class HotelRef
    {
        [XmlAttribute("HotelCode")]
        public string HotelCode { get; set; } = string.Empty;
        public bool ShouldSerializeHotelCode() => !string.IsNullOrEmpty(HotelCode);
    }
}
