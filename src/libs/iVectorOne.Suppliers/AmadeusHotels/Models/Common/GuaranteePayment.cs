namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class GuaranteePayment
    {
        [XmlAttribute("PaymentCode")]
        public string PaymentCode { get; set; } = string.Empty;
    }
}
