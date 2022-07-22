namespace iVectorOne.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class HotelRef
    {
        [XmlAttribute("ChainCode")]
        public string ChainCode { get; set; } = string.Empty;
        public bool ShouldSerializeChainCode() => !string.IsNullOrEmpty(ChainCode);

        [XmlAttribute("HotelCode")]
        public string HotelCode { get; set; } = string.Empty;

        [XmlAttribute("HotelCityCode")]
        public string HotelCityCode { get; set; } = string.Empty;

        [XmlAttribute("HotelCodeContext")]
        public string HotelCodeContext { get; set; } = string.Empty;
        public bool ShouldSerializeHotelCodeContext() => !string.IsNullOrEmpty(HotelCodeContext);
    }
}
