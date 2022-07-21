namespace iVectorOne.Suppliers.AmadeusHotels.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using iVectorOne.Suppliers.AmadeusHotels.Models.Common;

    public class RoomStays
    {
        [XmlAttribute("MoreIndicator")]
        public string MoreIndicator { get; set; } = string.Empty;

        [XmlElement("RoomStay")]
        public List<RoomStay> RoomStayList { get; set; } = new();
    }
}