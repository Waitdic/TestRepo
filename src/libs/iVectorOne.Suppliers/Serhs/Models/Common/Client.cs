namespace iVectorOne.Suppliers.Serhs.Models.Common
{
    using System.Xml.Serialization;

    public class Client
    {
        [XmlAttribute("code")]
        public string Code { get; set; } = string.Empty;

        [XmlAttribute("branch")]
        public string Branch { get; set; } = string.Empty;

        [XmlAttribute("password")]
        public string? Password { get; set; }

        [XmlAttribute("tradingGroup")]
        public string TradingGroup { get; set; } = string.Empty;
    }
}