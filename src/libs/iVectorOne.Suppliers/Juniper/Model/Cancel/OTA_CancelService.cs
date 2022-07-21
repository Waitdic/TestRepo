namespace ThirdParty.CSSuppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class OTA_CancelService
    {
        [XmlElement("OTA_CancelRQ")]
        public OTA_CancelRQ CancelRequest { get; set; } = new();
    }
}