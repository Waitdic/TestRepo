namespace ThirdParty.CSSuppliers.OceanBeds.Models.Common
{
    public class Room
    {
        public string Id { get; set; } = string.Empty;

        public decimal NetPrice { get; set; }

        public string Code { get; set; } = string.Empty;

        public string OfferText { get; set; } = string.Empty;

        public int MaxAdults { get; set; }

        public int MaxChildren { get; set; }

        public string Name { get; set; } = string.Empty;

        public int BedRoom { get; set; }
    }
}
