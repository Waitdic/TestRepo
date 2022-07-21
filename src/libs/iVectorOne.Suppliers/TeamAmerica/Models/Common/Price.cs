namespace ThirdParty.CSSuppliers.TeamAmerica.Models
{
    using System.Xml.Serialization;

    public class Price
    {
        [XmlElement("Occupancy")]
        public string Occupancy { get; set; } = string.Empty;

        [XmlElement("AdultPrice")]
        public string AdultPrice { get; set; } = string.Empty;
    }
}
