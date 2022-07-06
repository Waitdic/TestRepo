namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class Traveller
    {
        [XmlElement("surname")]
        public string Surname { get; set; } = string.Empty;

        [XmlElement("quantity")]
        public int Quantity { get; set; }
    }
}
