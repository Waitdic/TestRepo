namespace iVectorOne.Suppliers.Models.Yalago
{
#pragma warning disable CS8618

    class YalagoCancellationRequest
    {
        public string BookingRef { get; set; }
        public ExpectedCharge expectedCharge { get; set; }
        public class ExpectedCharge
        {
            public Charge charge { get; set; }
        }
        public class Charge
        {
            public decimal Amount { get; set; }
            public string Currency { get; set; }
        }
    }
}