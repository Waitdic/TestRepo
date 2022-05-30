namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    using System.Xml.Serialization;

    public class OTA_CancelServiceResponse
    {

        [XmlElement("OTA_CancelRS")]
        public OTA_CancelRS OTA_CancelRS { get; set; } = new();
    }
}
