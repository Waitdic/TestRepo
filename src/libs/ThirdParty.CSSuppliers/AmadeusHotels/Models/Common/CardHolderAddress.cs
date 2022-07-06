namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class CardHolderAddress
    {
        [XmlElement("addressDetails")]
        public AddressDetails AddressDetails { get; set; } = new();

        [XmlElement("city")]
        public string City { get; set; } = string.Empty;

        [XmlElement("zipCode")]
        public string ZipCode { get; set; } = string.Empty;
    }
}
