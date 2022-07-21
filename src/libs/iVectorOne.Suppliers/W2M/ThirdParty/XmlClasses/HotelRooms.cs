using System.Collections.Generic;
using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "HotelRooms")]
    public class HotelRooms
    {
        [XmlElement(ElementName = "HotelRoom")]
        public List<HotelRoom> HotelRoomList { get; set; } = new List<HotelRoom>();
    }
}
