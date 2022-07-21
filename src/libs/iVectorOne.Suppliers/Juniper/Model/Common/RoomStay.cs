namespace iVectorOne.CSSuppliers.Juniper.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    public class RoomStay
    {
        public RoomStay() { }

        [XmlArray("RatePlans")]
        [XmlArrayItem("RatePlan")]
        public List<RatePlan> RatePlans { get; set; } = new();

        [XmlArray("RoomTypes")]
        [XmlArrayItem("RoomType")]
        public List<RoomType> RoomTypes { get; set; } = new();

        [XmlArray("RoomRates")]
        [XmlArrayItem("RoomRate")]
        public List<RoomRate> RoomRates { get; set; } = new();
        public bool ShouldSerializeRoomRates() => RoomRates != null && RoomRates.Any();

        [XmlElement("TimeSpan")]
        public StayTimeSpan TimeSpan { get; set; } = new();

        [XmlElement("BasicPropertyInfo")]
        public BasicPropertyInfo BasicPropertyInfo { get; set; } = new();

        [XmlElement("Total")]
        public RateTotal Total { get; set; } = new();

        [XmlElement("TPA_Extensions")]
        public RoomStayExtension RoomStayExtension { get; set; } = new();

        [XmlArray("Comments")]
        [XmlArrayItem("Comment")]
        public List<Comment> Comments { get; set; } = new();
    }
}