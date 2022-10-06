namespace iVectorOne.Suppliers.Italcamel.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class PrebookRoom
    {
        public string UID { get; set; } = string.Empty;

        [XmlElement("ISWIN")]
        public bool IsWin { get; set; }

        [XmlElement("ISDUS")]
        public bool IsDus { get; set; }

        [XmlArray("PASSENGERS")]
        [XmlArrayItem("PASSENGER")]
        public Passenger[] Passengers { get; set; } = Array.Empty<Passenger>();

        [XmlElement("BOARD")]
        public Board Board { get; set; } = new();

        [XmlArray("SERVICES")]
        [XmlArrayItem("SERVICE")]
        public Service[] Services { get; set; } = Array.Empty<Service>();

        [XmlElement("AMOUNT")]
        public decimal Amount { get; set; }
        public bool ShouldSerializeAmount() => Amount != 0;

        [XmlElement("SPECIALOFFER")]
        public SpecialOffer? SpecialOffer { get; set; }
        public bool ShouldSerializeSpecialOffer() => SpecialOffer != null;

        [XmlElement("EXTRA")]
        public Extra? Extra { get; set; }
        public bool ShouldSerializeExtra() => Extra != null;
    }
}
