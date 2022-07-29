namespace iVectorOne.Suppliers.ATI.Models
{
    using System.Xml.Serialization;
    using iVectorOne.Suppliers.ATI.Models.Common;

    public class AtiCancellationRequest : SoapContent
    {
        [XmlAttribute("Version")]
        public string Version { get; set; } = string.Empty;

        public UniqueId UniqueID { get; set; } = new();
    }
}
