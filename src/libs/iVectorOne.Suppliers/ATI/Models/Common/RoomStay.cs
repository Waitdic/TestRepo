namespace iVectorOne.CSSuppliers.ATI.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class RoomStay
    {
        [XmlArray("RoomTypes")]
        [XmlArrayItem("RoomType")]
        public RoomType[] RoomTypes { get; set; } = Array.Empty<RoomType>();

        [XmlArray("RoomRates")]
        [XmlArrayItem("RoomRate")]
        public RoomRate[] RoomRates { get; set; } = Array.Empty<RoomRate>();

        public Total Total { get; set; } = new();

        [XmlArray("CancelPenalties")]
        [XmlArrayItem("CancelPenalty")]
        public CancelPenalty[] CancelPenalties { get; set; } = Array.Empty<CancelPenalty>();

        [XmlArray("GuestCounts")]
        [XmlArrayItem("GuestCount")]
        public GuestCount[] GuestCounts { get; set; } = Array.Empty<GuestCount>();

        public TimeSpan TimeSpan { get; set; } = new();

        public BasicPropertyInfo BasicPropertyInfo { get; set; } = new();
    }
}
