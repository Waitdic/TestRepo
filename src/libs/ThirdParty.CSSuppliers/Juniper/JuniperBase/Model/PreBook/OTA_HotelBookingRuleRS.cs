namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    using System.Xml.Serialization;

    public class OTA_HotelBookingRuleRS
    {
        [XmlElement("RuleMessage")]
        public PreBookRsRuleMessage RuleMessage { get; set; } = new();
    }
}
