#nullable disable warnings

namespace iVectorOne.CSSuppliers.HotelBedsV2
{
    public class PaymentData
    {
        public PaymentCard paymentCard { get; set; }
        public ContactData contactData { get; set; }
    }

    public class PaymentCard
    {
        public string cardHolderName { get; set; }
        public string cardType { get; set; }
        public string cardNumber { get; set; }
        public string expiryDate { get; set; }
        public string cardCVV { get; set; }

    }
    public class ContactData
    {
        public string email { get; set; }
        public string phoneNumber { get; set; }
    }
}

#nullable restore warnings