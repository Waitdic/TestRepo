namespace iVectorOne.Suppliers.Hotelston.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Room
    {
        [XmlAttribute("adults")]
        public int Adults { get; set; }

        [XmlAttribute("children")]
        public int Children { get; set; }

        [XmlElement("childAge")]
        public int[] ChildAges { get; set; } = Array.Empty<int>();

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