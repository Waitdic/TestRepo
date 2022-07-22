namespace iVectorOne.CSSuppliers.Netstorming.Models
{
    using System.Xml.Serialization;
    using iVectorOne.CSSuppliers.Netstorming.Models.Common;

    [XmlRoot("envelope")]
    public class NetstormingAvailabilityResponse : EnvelopeBase
    {
        [XmlElement("response")]
        public Response Response { get; set; } = new();
    }
}