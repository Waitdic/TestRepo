namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class AcceptedPayment
    {
        [XmlAttribute("RPH")]
        public int RPH { get; set; }

        public PaymentCard PaymentCard { get; set; } = new();
        public bool ShouldSerializePaymentCard() => !string.IsNullOrEmpty(PaymentCard?.CardNumber ?? "");

        public DirectBill DirectBill { get; set; } = new();
        public bool ShouldSerializeDirectBill => !string.IsNullOrEmpty(DirectBill?.DirectBillID ?? "");
    }
}
