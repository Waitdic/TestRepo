namespace ThirdParty.CSSuppliers.NetStorming.Models.Common
{
    using System.Xml.Serialization;

    public class QueryDate
    {
        [XmlAttribute("date")]
        public string Date { get; set; } = string.Empty;
    }
}