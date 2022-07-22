namespace iVectorOne.CSSuppliers.Jumbo.Models
{
    public class Price
    {
        public Amount amount { get; set; } = new Amount();

        public string boardTypeCode { get; set; }

        public RoomPrice roomPrices { get; set; } = new();
    }
}