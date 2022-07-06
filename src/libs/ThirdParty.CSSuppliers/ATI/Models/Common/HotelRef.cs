namespace ThirdParty.CSSuppliers.ATI.Models.Common
{
    using System.Xml.Serialization;

    public class HotelRef
    {
        [XmlAttribute("HotelCode")]
        public string HotelCode { get; set; } = string.Empty;

        public bool ShouldSerializeHotelCode() => !string.IsNullOrEmpty(HotelCode);

        [XmlAttribute("HotelCityCode")]
        public string HotelCityCode { get; set; } = string.Empty;

        public bool ShouldSerializeHotelCityCode() => !string.IsNullOrEmpty(HotelCityCode);
    }
}
