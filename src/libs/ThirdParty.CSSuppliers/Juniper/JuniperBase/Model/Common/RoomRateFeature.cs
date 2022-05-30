using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    public class RoomRateFeature
    {
        public RoomRateFeature() { }

        [XmlAttribute("RoomViewCode")]
        public string RoomViewCode { get; set; } = string.Empty;

        [XmlElement("Description")]
        public RateDescription Description { get; set; } = new();
    }
}
