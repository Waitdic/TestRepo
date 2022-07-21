namespace ThirdParty.CSSuppliers.TBOHolidays.Models
{
    using Common;

    public class AvailabilityAndPricingResponse : SoapContent
    {
        public bool CancellationPoliciesAvailable { get; set; }

        public Status Status { get; set; } = new();

        public PriceVerification PriceVerification { get; set; } = new();
    }
}