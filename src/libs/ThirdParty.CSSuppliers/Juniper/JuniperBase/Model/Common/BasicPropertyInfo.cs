namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    using System.Xml.Serialization;

    public class BasicPropertyInfo
    {
        public BasicPropertyInfo() { }

        [XmlAttribute("HotelCode")]
        public string HotelCode { get; set; } = string.Empty;
        public bool ShouldSerializeHotelCode() => !string.IsNullOrEmpty(HotelCode);

        [XmlAttribute("HotelName")]
        public string HotelName { get; set; } = string.Empty;
        public bool ShouldSerializeHotelName() => !string.IsNullOrEmpty(HotelName);
    }
}
