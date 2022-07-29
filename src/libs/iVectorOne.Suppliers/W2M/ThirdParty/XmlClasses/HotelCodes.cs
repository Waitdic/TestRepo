using System.Collections.Generic;
using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "HotelCodes")]
    public class HotelCodes
    {
        public HotelCodes(List<string> hotelCodeList)
        {
            HotelCodeList = hotelCodeList;
        }

        public HotelCodes()
        {
        }

        [XmlElement(ElementName = "HotelCode")]
        public List<string> HotelCodeList { get; set; }
    }
}
