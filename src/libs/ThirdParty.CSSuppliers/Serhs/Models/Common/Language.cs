namespace ThirdParty.CSSuppliers.Serhs.Models.Common
{
    using System.Xml.Serialization;

    public class Language
    {
        [XmlAttribute("code")]
        public string? Code { get; set; }
    }
}