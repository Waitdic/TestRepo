namespace ThirdParty.CSSuppliers.AmadeusHotels.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using ThirdParty.CSSuppliers.AmadeusHotels.Models.Common;

    public class RoomStays
    {
        [XmlAttribute("MoreIndicator")]
        public string MoreIndicator { get; set; } = string.Empty;

        [XmlElement("RoomStay")]
        public List<RoomStay> RoomStayList { get; set; } = new();
    }
}