namespace ThirdParty.CSSuppliers.iVectorChannelManager.Models
{
    public partial class CancelRequest
    {

        public BookingLogin LoginDetails { get; set; }

        public string BookingReference { get; set; }
        public decimal CancellationCost { get; set; }
        public string LeadGuestFirstName { get; set; }
        public string LeadGuestLastName { get; set; }
        public GuestConfiguration Guests { get; set; }

        public partial class GuestConfiguration
        {
            public int Adults { get; set; }
            public int Children { get; set; }
            public int Infants { get; set; }
        }

    }
}