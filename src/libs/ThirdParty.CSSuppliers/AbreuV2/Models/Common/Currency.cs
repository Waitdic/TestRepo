namespace ThirdParty.CSSuppliers.AbreuV2.Models
{
    using System.Xml.Serialization;

    public class Currency
    {
        [XmlAttribute("Code")]
        public string Code { get; set; } = string.Empty;
    }
}
