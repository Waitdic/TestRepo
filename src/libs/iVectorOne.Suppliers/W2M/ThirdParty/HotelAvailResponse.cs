using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618

    [XmlRoot(ElementName = "HotelAvailResponse")]
    public class HotelAvailResponse
    {
        public HotelAvailResponse()
        {
        }

        [XmlElement(ElementName = "AvailabilityRS")]
        public AvailabilityRS AvailabilityRS { get; set; }
    }
}
