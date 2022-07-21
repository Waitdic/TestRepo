namespace ThirdParty.CSSuppliers.BedsWithEase.Models.Common
{
    public class HotelInfo
    {
        public string Code { get; set; } = string.Empty;
        public string GiataCode { get; set; } = string.Empty;
        public string HotelGUID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Address Address { get; set; } = new();
        public ChildMinAge ChildMinAge { get; set; } = new();
        public Category Category { get; set; } = new();
        public OriginalCategory OriginalCategory { get; set; } = new();


    }
}
