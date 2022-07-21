namespace iVectorOne.Suppliers.BedsWithEase.Models.Common
{
    public class Offer
    {
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float NetPrice { get; set; }
        public float GrossPrice { get; set; }

    }
}
