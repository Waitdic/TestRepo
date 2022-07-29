namespace iVectorOne.Suppliers.BedsWithEase.Models.Common
{
    public class HlRvRate
    {
        public float Price { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public string AgeType { get; set; } = string.Empty;
        public Margin Margin { get; set; } = new();
    }
}
