namespace iVectorOne.Suppliers.Models.WelcomeBeds
{
    using System.Xml.Serialization;

    public class RoomType
    {
        public RoomType() { }

        [XmlAttribute("RoomTypeCode")]
        public string RoomTypeCode { get; set; } = string.Empty;

        [XmlElement("RoomDescription")]
        public RoomDescription RoomDescription { set; get; } = new RoomDescription();
    }

}
