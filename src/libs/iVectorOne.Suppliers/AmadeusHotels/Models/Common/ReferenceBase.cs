namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class ReferenceBase
    {
        [XmlElement("type")]
        public string Type { get; set; } = string.Empty;

        [XmlElement("value")]
        public string Value { get; set; } = string.Empty;
    }
}
