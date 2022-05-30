namespace ThirdParty.CSSuppliers.Serhs.Models.Common
{
    using System.Xml.Serialization;

    public class CancelPolicy
    {
        [XmlAttribute("releaseDate")]
        public string ReleaseDate { get; set; } = string.Empty;

        [XmlAttribute("amount")]
        public string Amount { get; set; } = string.Empty;
    }
}
