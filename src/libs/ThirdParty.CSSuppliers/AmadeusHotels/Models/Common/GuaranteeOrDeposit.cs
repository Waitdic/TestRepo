namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class GuaranteeOrDeposit
    {
        [XmlElement("paymentInfo")]
        public PaymentInfo PaymentInfo { get; set; } = new();

        [XmlElement("groupCreditCardInfo")]
        public GroupCreditCardInfo? GroupCreditCardInfo { get; set; }
        public bool ShouldSerializeGroupCreditCardInfo() => GroupCreditCardInfo != null;
    }
}
