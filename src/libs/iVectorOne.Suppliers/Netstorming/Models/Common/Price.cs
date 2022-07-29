namespace iVectorOne.Suppliers.Netstorming.Models.Common
{
    using System.Xml.Serialization;

    public class Price
    {
        [XmlElement("roomprice")]
        public RoomPrice RoomPrice { get; set; } = new();

        [XmlElement("extrabedprice")]
        public RoomPrice ExtraBedPrice { get; set; } = new();

        [XmlElement("cotprice")]
        public RoomPrice CotPrice { get; set; } = new();

        [XmlAttribute("from")]
        public string From { get; set; } = string.Empty;

        [XmlAttribute("to")]
        public string To { get; set; } = string.Empty;
    }
}