using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618

    public class CheckAvailabilityHotelOption
    {
        public CheckAvailabilityHotelOption(string ratePlanCode)
        {
            RatePlanCode = ratePlanCode;
        }

        public CheckAvailabilityHotelOption()
        {
        }

        [XmlAttribute(AttributeName = "RatePlanCode")]
        public string RatePlanCode { get; set; }
    }
}