namespace ThirdParty.CSSuppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class OTA_HotelBookingRuleRS
    {
        [XmlElement("RuleMessage")]
        public PreBookRsRuleMessage RuleMessage { get; set; } = new();
    }
}