using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "HotelRequiredFields")]
    public class HotelRequiredFields
    {
        [XmlElement(ElementName = "HotelBooking")]
        public HotelBooking HotelBooking { get; set; }
    }
}
