namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class AmountPercent
    {
        [XmlAttribute("Percent")]
        public string Percent { get; set; } = string.Empty;

        [XmlAttribute("Amount")]
        public string Amount { get; set; } = string.Empty;

        [XmlAttribute("NmbrOfNights")]
        public string NmbrOfNights { get; set; } = string.Empty;
    }
}
