namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class RoomType
    {
        [XmlAttribute("NumberOfUnits")]
        public int NumberOfUnits { get; set; }

        [XmlAttribute("RoomTypeCode")]
        public string RoomTypeCode { get; set; } = string.Empty;

        [XmlElement("RoomDescription")]
        public RoomDescription RoomDescription { set; get; } = new();
    }

}
