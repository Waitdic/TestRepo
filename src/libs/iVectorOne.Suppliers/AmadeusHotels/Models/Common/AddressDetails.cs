namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class AddressDetails
    {
        [XmlElement("format")]
        public string Format { get; set; } = string.Empty;

        [XmlElement("line1")]
        public string Line1 { get; set; } = string.Empty;
    }
}
