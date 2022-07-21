namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class RoomStay
    {
        [XmlArray("RoomRates")]
        [XmlArrayItem("RoomRate")]
        public RoomRate[] RoomRates { get; set; } = Array.Empty<RoomRate>();

        [XmlArray("RoomTypes")]
        [XmlArrayItem("RoomType")]
        public RoomType[] RoomTypes { get; set; } = Array.Empty<RoomType>();

        [XmlArray("RatePlans")]
        [XmlArrayItem("RatePlan")]
        public RatePlan[] RatePlans { get; set; } = Array.Empty<RatePlan>();

        [XmlAttribute("RPH")]
        public string Rph { get; set; } = string.Empty;
    }
}
