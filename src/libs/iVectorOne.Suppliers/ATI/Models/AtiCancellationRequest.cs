namespace ThirdParty.CSSuppliers.ATI.Models
{
    using System.Xml.Serialization;
    using ThirdParty.CSSuppliers.ATI.Models.Common;

    public class AtiCancellationRequest : SoapContent
    {
        [XmlAttribute("Version")]
        public string Version { get; set; } = string.Empty;

        public UniqueId UniqueID { get; set; } = new();
    }
}
