namespace ThirdParty.CSSuppliers.NetStorming.Models.Common
{
    using System.Xml.Serialization;

    public class Availonly
    {
        [XmlAttribute("value")]
        public string Value { get; set; } = string.Empty;
    }
}