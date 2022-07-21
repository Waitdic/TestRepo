#nullable disable warnings
#pragma warning disable CA1819 // Properties should not return arrays
#pragma warning disable CA1034 // Nested types should not be visible

namespace iVectorOne.Suppliers.HotelBedsV2
{
    public class HotelBedsV2CreateBookingRequest
    {
        public Holder holder { get; set; }
        public Room[] rooms { get; set; }
        public string clientReference { get; set; }
        public string remark { get; set; }
        public int tolerance { get; set; }
        public PaymentData paymentData { get; set; }

        public class Holder
        {
            public string name { get; set; }
            public string surname { get; set; }
        }

        public class Room
        {
            public string rateKey { get; set; }
            public Pax[] paxes { get; set; }
        }

        public class Pax
        {
            public string roomId { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public string surname { get; set; }
        }
    }
}

#nullable restore warnings
#pragma warning restore CA1819 // Properties should not return arrays
#pragma warning restore CA1034 // Nested types should not be visible