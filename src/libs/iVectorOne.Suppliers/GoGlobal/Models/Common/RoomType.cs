namespace iVectorOne.Suppliers.GoGlobal.Models
{
    using System.Xml.Serialization;

    public class RoomType
    {
        [XmlAttribute("Adults")]
        public int Adults { get; set; }

        [XmlAttribute("Cots")]
        public int Cots { get; set; }
        public bool ShouldSerializeCots() => Cots > 0;


        [XmlElement("Room")]
        public BookRoom Room { get; set; } = new();
    }
}
