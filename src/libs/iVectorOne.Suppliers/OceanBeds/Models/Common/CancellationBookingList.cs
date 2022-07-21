namespace iVectorOne.Suppliers.OceanBeds.Models.Common
{

    public class CancellationBookingList
    {
        public int Key { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BookingRef { get; set; } = string.Empty;
        public string ClientRef { get; set; } = string.Empty;
        public string ArrivalDate { get; set; } = string.Empty;
        public string DepartureDate { get; set; } = string.Empty;
        public string SellPrice { get; set; } = string.Empty;
        public string CancellationCharge { get; set; } = string.Empty;

    }
}
