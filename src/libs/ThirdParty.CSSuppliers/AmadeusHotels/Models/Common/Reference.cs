namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class Reference
    {
        [XmlElement("qualifier")]
        public string Qualifier { get; set; } = string.Empty;

        [XmlElement("number")]
        public int Number { get; set; }
    }
}
