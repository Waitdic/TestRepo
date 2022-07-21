namespace iVectorOne.Suppliers.Hotelston.Models
{
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("CheckAvailabilityResponse")]
    public class CheckAvailabilityResponse : SoapContent
    {
        [XmlElement("success")]
        public bool Success { get; set; }

        [XmlElement("hotel")]
        public Hotel Hotel { get; set; }
    }
}
