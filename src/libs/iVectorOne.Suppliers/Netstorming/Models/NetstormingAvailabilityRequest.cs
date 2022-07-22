namespace iVectorOne.CSSuppliers.Netstorming.Models
{
    using System.Xml.Serialization;
    using iVectorOne.CSSuppliers.Netstorming.Models.Common;

    [XmlRoot("envelope")]
    public class NetstormingAvailabilityRequest : EnvelopeBase
    {
        [XmlElement("query")]
        public Query Query { get; set; } = new();
    }
}