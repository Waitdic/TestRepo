namespace ThirdParty.CSSuppliers.BedsWithEase.Models.Common
{
    public class Address
    {
        public Airport Airport { get; set; } = new();
        public Resort Resort { get; set; } = new();
    }
}
