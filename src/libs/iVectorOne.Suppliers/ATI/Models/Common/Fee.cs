namespace iVectorOne.Suppliers.ATI.Models.Common
{
    public class Fee
    {
        public decimal Amount { get; set; }

        public string ChargeFrequency { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string CurrencyCode { get; set; } = string.Empty;

        public bool Included { get; set; }

        public bool Mandatory { get; set; }
    }
}
