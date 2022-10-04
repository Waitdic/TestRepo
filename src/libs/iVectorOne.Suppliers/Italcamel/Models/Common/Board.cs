namespace iVectorOne.Suppliers.Italcamel.Models.Common
{
    using System.Xml.Serialization;

    public class Board
    {
        [XmlElement("AMOUNT")]
        public decimal Amount { get; set; }

        [XmlElement("ACRONYM")]
        public string Acronym { get; set; } = string.Empty;

        public string UID { get; set; } = string.Empty;
    }
}
