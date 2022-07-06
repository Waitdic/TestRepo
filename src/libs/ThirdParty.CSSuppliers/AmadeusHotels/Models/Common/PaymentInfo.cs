namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class PaymentInfo
    {
        [XmlElement("paymentDetails")]
        public PaymentDetails PaymentDetails { get; set; } = new();
    }
}
