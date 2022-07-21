namespace iVectorOne.Suppliers.Juniper.Model
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RoomRate
    {
        public RoomRate() { }

        [XmlAttribute("AvailabilityStatus")]
        public string AvailabilityStatus { get; set; } = string.Empty;

        [XmlAttribute("RatePlanCode")]
        public string RatePlanCode { get; set; } = string.Empty;

        [XmlAttribute("RatePlanCategory")]
        public string RatePlanCategory { get; set; } = string.Empty;

        [XmlArray("Rates")]
        [XmlArrayItem("Rate")]
        public List<Rate> Rates { get; set; } = new();

        [XmlArray("Features")]
        [XmlArrayItem("Feature")]
        public List<RoomRateFeature> Features { get; set; } = new();

        [XmlElement("Total")]
        public RateTotal Total { get; set; } = new();

        [XmlElement("TPA_Extensions")]
        public RoomRateExtension RoomRateExtension { get; set; } = new();
    }
}