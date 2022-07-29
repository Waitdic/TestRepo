namespace iVectorOne.Suppliers.Hotelston.Models.Common
{
    using System.Xml.Serialization;

    public class RoomType
    {
        [XmlAttribute("id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("nameEn")]
        public string NameEn { get; set; } = string.Empty;
    }
}