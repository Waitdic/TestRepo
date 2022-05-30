namespace ThirdParty.CSSuppliers.NetStorming.Models.Common
{
    using System.Xml.Serialization;

    public class EnvelopeBase
    {
        [XmlElement("header")]
        public Header Header { get; set; } = new();
    }
}