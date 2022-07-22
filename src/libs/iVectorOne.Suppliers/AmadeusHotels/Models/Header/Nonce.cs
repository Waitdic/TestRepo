namespace iVectorOne.CSSuppliers.AmadeusHotels.Models.Header
{
    using System.Xml.Serialization;

    public class Nonce
    {
        [XmlAttribute("EncodingType")]
        public string EncodingType { get; set; } = string.Empty;

        [XmlText]
        public string Base64Nonce { get; set; } = string.Empty;
    }
}
