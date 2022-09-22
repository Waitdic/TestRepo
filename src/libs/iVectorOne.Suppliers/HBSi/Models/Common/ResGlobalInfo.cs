namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    public class ResGlobalInfo
    {
        [XmlArray("HotelReservationIDs")]
        [XmlArrayItem("HotelReservationID")]
        public List<HotelReservationId> HotelReservationIds { get; set; } = new();
        public bool ShouldSerializeHotelReservationIds() => HotelReservationIds.Any();

        [XmlArray("Comments")]
        [XmlArrayItem("Comment")]
        public List<Comment> Comments { get; set; } = new();

        [XmlElement("DepositPayments")]
        public DepositPayments DepositPayments { get; set; } = new();
        public bool ShouldSerializeDepositPayments() => DepositPayments.RequiredPayment.AcceptedPayments.Any();
    }

    public class DepositPayments
    {
        public RequiredPayment RequiredPayment { get; set; } = new();
    }

    public class RequiredPayment
    {
        [XmlArray("AcceptedPayments")]
        [XmlArrayItem("AcceptedPayment")]
        public List<AcceptedPayment> AcceptedPayments { get; set; } = new();

        public AmountPercent AmountPercent { get; set; } = new();
        public Deadline Deadline { get; set; } = new();
    }

    public class AcceptedPayment
    {
        [XmlAttribute("RPH")]
        public int RPH { get; set; }

        public PaymentCard PaymentCard { get; set; } = new();
        public bool ShouldSerializePaymentCard() => !string.IsNullOrEmpty(PaymentCard?.CardNumber ?? "");

        public DirectBill DirectBill { get; set; } = new();
        public bool ShouldSerializeDirectBill => !string.IsNullOrEmpty(DirectBill?.DirectBillID ?? "");
    }

    public class PaymentCard
    {
        public string CardType { get; set; } = string.Empty;
        public string CardCode { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string ExpireDate { get; set; } = string.Empty;
    }

    public class DirectBill
    {
        public string DirectBillID { get; set; } = string.Empty;
    }


    public class Comment
    {
        public Comment() { }

        [XmlElement("Text", Namespace = "http://www.opentravel.org/OTA/2003/05")]
        public string Text { get; set; } = string.Empty;
    }
}
