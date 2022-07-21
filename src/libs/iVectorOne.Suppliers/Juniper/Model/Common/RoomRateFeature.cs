namespace iVectorOne.Suppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class RoomRateFeature
    {
        public RoomRateFeature() { }

        [XmlAttribute("RoomViewCode")]
        public string RoomViewCode { get; set; } = string.Empty;

        [XmlElement("Description")]
        public RateDescription Description { get; set; } = new();
    }
}