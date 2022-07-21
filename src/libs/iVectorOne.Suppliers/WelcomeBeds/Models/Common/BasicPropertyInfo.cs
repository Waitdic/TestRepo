namespace ThirdParty.CSSuppliers.Models.WelcomeBeds
{
    using System.Xml.Serialization;

    public class BasicPropertyInfo
    {
        public BasicPropertyInfo() { }

        [XmlAttribute("HotelCode")]
        public string HotelCode { get; set; } = string.Empty;

        [XmlAttribute("HotelName")]
        public string HotelName { get; set; } = string.Empty;
    }

}
