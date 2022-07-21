namespace iVectorOne.Suppliers.Models.Altura
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Rate
    {
        public Rate() { }

        [XmlArray("Rooms")]
        [XmlArrayItem("Room")]
        public List<Room> Rooms { get; set; } = new();

        [XmlAttribute("Currency")]
        public string Currency { get; set; } = string.Empty;

        [XmlAttribute("Board")]
        public string Board { get; set; } = string.Empty;

        [XmlAttribute("id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("NoRefundable")]
        public string NoRefundable { get; set; } = string.Empty;

        [XmlAttribute("TotalPrice")]
        public string TotalPrice { get; set; } = string.Empty;

        [XmlArray("CancellationPrices")]
        [XmlArrayItem("Price")]
        public List<Price> CancellationPrices { get; set; } = new();
    }

    public class Price
    {
        public Price() { }

        [XmlAttribute("timeframe")]
        public string Timeframe { get; set; } = string.Empty;

        [XmlAttribute("unit")]
        public string Unit { get; set; } = string.Empty;

        [XmlAttribute("currency")]
        public string Currency { get; set; } = string.Empty;

        [XmlText]
        public string TotalPrice { get; set; } = string.Empty;
    }
}
