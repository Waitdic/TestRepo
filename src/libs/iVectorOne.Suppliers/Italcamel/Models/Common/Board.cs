namespace iVectorOne.Suppliers.Italcamel.Models.Common
{
    using System.Xml.Serialization;
    using iVectorOne.Suppliers.Italcamel.Models.Envelope;

    public class Board
    {
        public string UID { get; set; } = string.Empty;

        [XmlElement("ACRONYM")]
        public string Acronym { get; set; } = string.Empty;
        public bool ShouldSerializeAcronym() => !string.IsNullOrEmpty(Acronym);

        [XmlElement("AMOUNT")]
        public decimal Amount { get; set; }
        public bool ShouldSerializeAmount() => Amount != 0;
    }
}
