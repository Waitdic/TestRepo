namespace iVectorOne.Suppliers.Miki.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class BookingBookRequest
    {
        [XmlAttribute("currencyCode")]
        public string CurrencyCode { get; set; } = string.Empty;

        [XmlAttribute("paxNationality")]
        public string PaxNationality { get; set; } = string.Empty;

        [XmlElement("clientRef")]
        public string ClientRef { get; set; } = string.Empty;

        [XmlArray("items")]
        [XmlArrayItem("item")]
        public Item[] Items { get; set; } = Array.Empty<Item>();
    }
}
