namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class RoomType
    {
        [XmlAttribute("RoomType")]
        public string RoomTypeAttribute { get; set; } = string.Empty;

        [XmlAttribute("IsConverted")]
        public bool IsConverted { get; set; }

        [XmlAttribute]
        public string RoomTypeCode { get; set; } = string.Empty;
    }
}
