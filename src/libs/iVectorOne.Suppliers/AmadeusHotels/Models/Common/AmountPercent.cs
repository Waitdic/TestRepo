namespace iVectorOne.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class AmountPercent
    {
        [XmlAttribute("Percent")]
        public decimal Percent { get; set; }

        [XmlAttribute("NmbrOfNights")]
        public int NmbrOfNights { get; set; }

        [XmlAttribute("Amount")]
        public decimal Amount { get; set; }
    }
}
