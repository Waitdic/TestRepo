namespace ThirdParty.CSSuppliers.NetStorming.Models.Common
{
    using System.Xml.Serialization;

    public class QueryCategory
    {
        [XmlAttribute("code")]
        public string Code { get; set; } = string.Empty;
    }
}