namespace ThirdParty.CSSuppliers.BedsWithEase.Models.Common
{
    public class MarketCountries
    {
        public Mode Mode { get; set; }
        public Countries Countries { get; set; } = new();

    }

    public enum Mode
    {
        INCLUDE,
        EXCLUDE
    }
}
