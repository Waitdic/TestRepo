using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "HotelElement")]
    public class HotelElement
    {
        public HotelElement(string bookingCode, RelativePaxesDistribution relativePaxesDistribution,
            HotelBookingInfo hotelBookingInfo, Comments comments)
        {
            BookingCode = bookingCode;
            RelativePaxesDistribution = relativePaxesDistribution;
            HotelBookingInfo = hotelBookingInfo;
            Comments = comments;
        }

        public HotelElement()
        {
        }

        [XmlElement(ElementName = "BookingCode")]
        public string BookingCode { get; set; }
        [XmlElement(ElementName = "RelPaxesDist")]
        public RelativePaxesDistribution RelativePaxesDistribution { get; set; }
        [XmlElement(ElementName = "HotelBookingInfo")]
        public HotelBookingInfo HotelBookingInfo { get; set; }
        [XmlElement(ElementName = "Comments")]
        public Comments Comments { get; set; }
    }
}
