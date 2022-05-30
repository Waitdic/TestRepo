namespace ThirdParty.CSSuppliers.NetStorming.Models
{
    using System.Xml.Serialization;
    using ThirdParty.CSSuppliers.NetStorming.Models.Common;

    [XmlRoot("envelope")]
    public class NetstormingAvailabilityResponse : EnvelopeBase
    {
        [XmlElement("response")]
        public Response Response { get; set; } = new();
    }
}