namespace iVectorOne.Suppliers.TBOHolidays.Models.Common
{
    using System.Xml.Serialization;

    public class PaymentInfo
    {
        [XmlAttribute("VoucherBooking")]
        public bool VoucherBooking { get; set; }

        [XmlAttribute("PaymentModeType")]
        public PaymentModeType PaymentModeType { get; set; }
    }
}
