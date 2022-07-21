namespace iVectorOne.Suppliers.Models.WelcomeBeds
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RoomRate
    {
        public RoomRate() { }

        [XmlElement("RoomRateDescription")]
        public List<RoomRateDescription> Descriptions { get; set; } = new List<RoomRateDescription>();

        [XmlArray("Rates")]
        [XmlArrayItem("Rate")]
        public List<Rate> Rates { get; set; } = new List<Rate>();

        [XmlArray("GuestCounts")]
        [XmlArrayItem("GuestCount")]
        public List<GuestCount> GuestCounts { get; set; } = new List<GuestCount>();

        [XmlAttribute("RoomTypeCode")]
        public string RoomTypeCode { get; set; } = string.Empty;

        [XmlAttribute("RatePlanCode")]
        public string RatePlanCode { get; set; } = string.Empty;

        [XmlAttribute("InvBlockCode")]
        public string InvBlockCode { get; set; } = string.Empty;

        [XmlAttribute("PromotionCode")]
        public string PromotionCode { get; set; } = string.Empty;

        [XmlAttribute("AvailabilityStatus")]
        public string AvailabilityStatus { get; set; } = string.Empty;
    }

}
