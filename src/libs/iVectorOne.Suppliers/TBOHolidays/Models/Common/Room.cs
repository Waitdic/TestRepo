namespace iVectorOne.CSSuppliers.TBOHolidays.Models.Common
{
    using System.Xml.Serialization;

    public class Room
    {
        [XmlAttribute("PRBID")]
        public int PRBID { get; set; }

        public int[] RoomIndex { get; set; }
    }
}
