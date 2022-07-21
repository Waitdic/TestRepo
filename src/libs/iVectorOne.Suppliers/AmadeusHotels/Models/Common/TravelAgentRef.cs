namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class TravelAgentRef
    {
        [XmlElement("status")]
        public string Status { get; set; } = string.Empty;

        [XmlElement("reference")]
        public ReferenceBase Reference { get; set; } = new();
    }
}
