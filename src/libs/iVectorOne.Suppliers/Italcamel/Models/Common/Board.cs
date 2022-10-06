namespace iVectorOne.Suppliers.Italcamel.Models.Common
{
    using System.Xml.Serialization;

    public class Board
    {
        [XmlElement("AMOUNT")]
        public decimal Amount { get; set; }
        public bool ShouldSerializeAmount() => Amount != 0;

        [XmlElement("ACRONYM")]
        public string Acronym { get; set; } = string.Empty;
        public bool ShouldSerializeAcronym() => !string.IsNullOrEmpty("Acronym");

        public string UID { get; set; } = string.Empty;
    }
}
