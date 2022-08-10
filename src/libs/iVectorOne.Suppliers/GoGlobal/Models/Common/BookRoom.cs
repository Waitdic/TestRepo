namespace iVectorOne.Suppliers.GoGlobal.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class BookRoom
    {
        [XmlAttribute("RoomID")]
        public int RoomID { get; set; }

        [XmlElement("PersonName")]
        public List<PersonName> Guests { get; set; } = new();

        [XmlElement("ExtraBed")]
        public List<ExtraBed> ExtraBeds { get; set; } = new();
    }


}
