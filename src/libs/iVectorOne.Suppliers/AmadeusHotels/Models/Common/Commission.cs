namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class Commission
    {
        [XmlAttribute]
        public string StatusType { get; set; } = string.Empty;

        [XmlAttribute]
        public decimal Percent { get; set; }
    }
}