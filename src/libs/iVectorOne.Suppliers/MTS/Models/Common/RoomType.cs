namespace iVectorOne.Suppliers.MTS.Models.Common
{
    using System.Xml.Serialization;

    public class RoomType
    {
        [XmlElement()]
        public Description? RoomDescription { get; set; }
        public bool ShouldSerializeRoomDescription() => RoomDescription != null;

        [XmlAttribute("RoomTypeCode")]
        public string Code = string.Empty;
    }
}
