namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class MessageErrorText
    {
        [XmlElement("text")]
        public string Text { get; set; } = string.Empty;
    }
}
