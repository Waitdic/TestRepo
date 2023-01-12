namespace iVectorOne.Suppliers.PremierInn.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class RoomDetails
    {
        [XmlAttribute]
        public int Number { get; set; }

        [XmlElement("GuestName")]
        public GuestName[] GuestName { get; set; } = Array.Empty<GuestName>();
        public bool ShouldSerializeGuestName() => GuestName != Array.Empty<GuestName>();

        [XmlAttribute]
        public string RoomType { get; set; } = string.Empty;
        public bool ShouldSerializeRoomType() => !string.IsNullOrEmpty(RoomType);

        [XmlAttribute]
        public int Adults { get; set; }
        public bool ShouldSerializeAdults() => GuestName == Array.Empty<GuestName>();

        [XmlAttribute]
        public int Children { get; set; }
        public bool ShouldSerializeChildren() => GuestName == Array.Empty<GuestName>();

        [XmlAttribute]
        public string Cots { get; set; } = string.Empty;
        public bool ShouldSerializeCots() => !string.IsNullOrEmpty(Cots);

        [XmlAttribute]
        public string Disabled { get; set; } = string.Empty;
        public bool ShouldSerializeDisabled() => !string.IsNullOrEmpty(Disabled);

        [XmlAttribute]
        public string Double { get; set; } = string.Empty;
        public bool ShouldSerializeDouble() => !string.IsNullOrEmpty(Double);

        public string Text { get; set; } = string.Empty;
        public bool ShouldSerializeText() => !string.IsNullOrEmpty(Text);

        public Rate? Rate { get; set; }
        public bool ShouldSerializeRate() => Rate != null;
    }
}
