namespace iVectorOne.CSSuppliers.BedsWithEase.Models.Common
{
    public class CancellationPolicy
    {
        public int FromDays { get; set; }

        public int ToDays { get; set; }

        public string From { get; set; } = string.Empty;

        public string To { get; set; } = string.Empty;

        public string HowRule { get; set; } = string.Empty;

        public string Amount { get; set; } = string.Empty;

        public string CurrencyCode { get; set; } = string.Empty;
    }
}