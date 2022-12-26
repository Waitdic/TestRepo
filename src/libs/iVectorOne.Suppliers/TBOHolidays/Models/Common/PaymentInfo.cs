namespace iVectorOne.Suppliers.TBOHolidays.Models.Common
{
    public class PaymentInfo
    {
        public string CvvNumber { get; set; } = string.Empty;

        public string CardNumber { get; set; } = string.Empty;

        public string CardExpirationMonth { get; set; } = string.Empty;

        public string CardHolderFirstName { get; set; } = string.Empty;

        public string CardHolderlastName { get; set; } = string.Empty;

        public decimal BillingAmount { get; set; }

        public string BillingCurrency { get; set; } = string.Empty;

        public CardHolderAddress CardHolderAddress { get; set; } = new();
    }
}
