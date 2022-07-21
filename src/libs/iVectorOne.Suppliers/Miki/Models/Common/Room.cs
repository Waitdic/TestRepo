namespace iVectorOne.Suppliers.Miki.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Room
    {
        [XmlElement("roomNo")]
        public int RoomNo { get; set; }

        [XmlAttribute("roomTypeCode")]
        public string RoomTypeCode { get; set; } = string.Empty;
        public bool ShouldSerializeRoomTypeCode() => !string.IsNullOrEmpty(RoomTypeCode);

        [XmlElement("rateIdentifier")]
        public string RateIdentifier { get; set; } = string.Empty;
        public bool ShouldSerializeRateIdentifier() => !string.IsNullOrEmpty(RateIdentifier);

        [XmlElement("roomTotalPrice")]
        public decimal RoomTotalPrice { get; set; }
        public bool ShouldSerializeRoomTotalPrice() => RoomTotalPrice != 0;

        [XmlArray("guests")]
        [XmlArrayItem("guest")]
        public Guest[] Guests { get; set; } = Array.Empty<Guest>();

        [XmlArray("cancellationPolicies")]
        [XmlArrayItem("cancellationPolicy")]
        public CancellationPolicy[] CancellationPolicies { get; set; } = Array.Empty<CancellationPolicy>();
        public bool ShouldSerializeCancellationPolicies() => CancellationPolicies != Array.Empty<CancellationPolicy>();
    }
}
