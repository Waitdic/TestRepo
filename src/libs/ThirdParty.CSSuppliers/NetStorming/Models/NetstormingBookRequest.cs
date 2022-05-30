namespace ThirdParty.CSSuppliers.NetStorming.Models
{
    using System.Xml.Serialization;
    using ThirdParty.CSSuppliers.NetStorming.Models.Common;

    [XmlRoot("envelope")]
    public class NetstormingBookRequest : EnvelopeBase
    {
        [XmlElement("query")]
        public Query Query { get; set; } = new();
    }
}