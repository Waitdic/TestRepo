namespace ThirdParty.CSSuppliers.Hotelston.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class BookRoom
    {
        [XmlElement("adult")]
        public Guest[] Adults { get; set; } = Array.Empty<Guest>();

        [XmlElement("child")]
        public Guest[] Children { get; set; } = Array.Empty<Guest>();

        [XmlElement("roomTypeId")]
        public string RoomTypeId { get; set; } = string.Empty;

        public bool ShouldSerializeRoomTypeId() => !string.IsNullOrEmpty(RoomTypeId);

        [XmlElement("roomId")]
        public string RoomId { get; set; } = string.Empty;

        public bool ShouldSerializeRoomId() => !string.IsNullOrEmpty(RoomId);

        [XmlElement("boardTypeId")]
        public string BoardTypeId { get; set; } = string.Empty;

        public bool ShouldSerializeBoardTypeId() => !string.IsNullOrEmpty(BoardTypeId);

        [XmlElement("price")]
        public decimal Price { get; set; }

        public bool ShouldSerializePrice() => Price != 0;
    }
}