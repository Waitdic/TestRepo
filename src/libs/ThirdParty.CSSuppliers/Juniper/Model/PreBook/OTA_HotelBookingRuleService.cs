namespace ThirdParty.CSSuppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class OTA_HotelBookingRuleService
    {
        [XmlElement("OTA_HotelBookingRuleRQ")]
        public OTA_HotelBookingRuleRQ HotelBookingRuleRQ { get; set; } = new();
    }
}