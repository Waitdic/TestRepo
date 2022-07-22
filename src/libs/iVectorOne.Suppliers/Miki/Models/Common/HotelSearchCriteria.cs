namespace iVectorOne.CSSuppliers.Miki.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class HotelSearchCriteria
    {
        [XmlAttribute("currencyCode")]
        public string CurrencyCode { get; set; } = string.Empty;

        [XmlAttribute("paxNationality")]
        public string PaxNationality { get; set; } = string.Empty;

        [XmlAttribute("languageCode")]
        public string LanguageCode { get; set; } = string.Empty;

        [XmlElement("destination")]
        public Destination Destination { get; set; } = new();

        [XmlElement("stayPeriod")]
        public StayPeriod StayPeriod { get; set; } = new();

        [XmlArray("rooms")]
        [XmlArrayItem("room")]
        public Room[] Rooms { get; set; } = Array.Empty<Room>();
    }
}
