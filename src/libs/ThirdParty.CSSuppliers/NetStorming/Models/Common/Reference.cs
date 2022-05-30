namespace ThirdParty.CSSuppliers.NetStorming.Models.Common
{
    using System.Xml.Serialization;

    public class Reference
    {
        [XmlAttribute("code")]
        public string Code { get; set; } = string.Empty;
    }
}