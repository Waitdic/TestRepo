namespace iVectorOne.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class PaymentDetails
    {
        [XmlElement("formOfPaymentCode")]
        public int FormOfPaymentCode { get; set; }

        [XmlElement("paymentType")]
        public int PaymentType { get; set; }

        [XmlElement("serviceToPay")]
        public int ServiceToPay { get; set; }
    }
}
