using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "HotelOption")]
    public class RequestHotelOption
    {
        public RequestHotelOption(string ratePlanCode)
        {
            RatePlanCode = ratePlanCode;
        }

        public RequestHotelOption()
        {
        }

        [XmlAttribute(AttributeName = "RatePlanCode")]
        public string RatePlanCode { get; set; }
    }
}
