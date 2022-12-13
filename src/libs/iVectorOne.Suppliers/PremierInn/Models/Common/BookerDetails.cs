namespace iVectorOne.Suppliers.PremierInn.Models.Common
{
    public class BookerDetails
    {
        public GuestName BookerName { get; set; } = new();

        public Address? BookerAddress { get; set; }
        public bool ShouldSerializeBookerAddress() => BookerAddress != null;
    }
}
