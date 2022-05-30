namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    using System.Xml.Serialization;

    public class OTA_HotelBookingRuleServiceResponse
    {
        [XmlElement("OTA_HotelBookingRuleRS")]
        public OTA_HotelBookingRuleRS HotelBookingRuleRS { get; set; } = new();
    }
}
