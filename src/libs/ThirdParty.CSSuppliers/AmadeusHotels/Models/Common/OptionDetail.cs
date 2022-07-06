namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class OptionDetail
    {
        [XmlElement("type")]
        public string Type { get; set; } = string.Empty;

        [XmlElement("freetext")]
        public string FreeText { get; set; } = string.Empty;
    }
}
