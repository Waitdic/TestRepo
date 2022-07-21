namespace iVectorOne.CSSuppliers.AbreuV2.Models
{
    using System.Xml.Serialization;

    public class HotelInfo
    {
        [XmlAttribute("HotelCode")]
        public string HotelCode { get; set; } = string.Empty;
    }
}
