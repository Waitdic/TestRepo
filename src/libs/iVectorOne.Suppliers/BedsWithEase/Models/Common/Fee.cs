namespace iVectorOne.Suppliers.BedsWithEase.Models.Common
{

    public class Fee
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public float Amount { get; set; }
        public LocalCurrency LocalCurrency { get; set; } = new();
        public string Scope { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;

    }
}
