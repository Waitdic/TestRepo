namespace ThirdParty.CSSuppliers.Netstorming.Models.Common
{
    using System.Xml.Serialization;

    public class RequestAgreement
    {
        [XmlAttribute("code")]
        public string Code { get; set; } = string.Empty;
    }
}