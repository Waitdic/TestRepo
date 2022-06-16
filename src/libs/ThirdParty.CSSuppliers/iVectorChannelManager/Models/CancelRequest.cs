namespace ThirdParty.CSSuppliers.iVectorChannelManager.Models
{
    public class CancelRequest
    {
        public BookingLogin LoginDetails { get; set; } = new();

        public string BookingReference { get; set; }
        public decimal CancellationCost { get; set; }
        public string LeadGuestFirstName { get; set; }
        public string LeadGuestLastName { get; set; }
        public GuestConfiguration Guests { get; set; }

        public class GuestConfiguration
        {
            public int Adults { get; set; }
            public int Children { get; set; }
            public int Infants { get; set; }
        }
    }
}