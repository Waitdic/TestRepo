namespace ThirdParty.CSSuppliers.NetStorming.Models.Common
{
    using System.Xml.Serialization;

    public class Header
    {
        [XmlElement("actor")]
        public string Actor { get; set; } = string.Empty;

        [XmlElement("user")]
        public string User { get; set; } = string.Empty;

        [XmlElement("password")]
        public string Password { get; set; } = string.Empty;

        [XmlElement("version")]
        public string Version { get; set; } = string.Empty;

        [XmlElement("timestamp")]
        public string Timestamp { get; set; } = string.Empty;
    }
}