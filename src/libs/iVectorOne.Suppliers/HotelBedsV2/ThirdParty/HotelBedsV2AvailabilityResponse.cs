#nullable disable warnings
#pragma warning disable CA1819 // Properties should not return arrays
#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1707 // Identifiers should not contain underscores

namespace iVectorOne.Suppliers.HotelBedsV2
{
    using System;

    public class HotelBedsV2AvailabilityResponse
    {
        public Auditdata auditData { get; set; } = new();
        public Hotels hotels { get; set; } = new();

        public class Auditdata
        {
            public string processTime { get; set; }
            public string timestamp { get; set; }
            public string requestHost { get; set; }
            public string serverId { get; set; }
            public string environment { get; set; }
            public string release { get; set; }
            public string token { get; set; }
            public string _internal { get; set; }
        }

        public class Hotels
        {
            public Hotel[] hotels { get; set; }
            public string checkIn { get; set; }
            public int total { get; set; }
            public string checkOut { get; set; }
        }

        public class Hotel
        {
            public int code { get; set; }
            public string name { get; set; }
            public string categoryCode { get; set; }
            public string categoryName { get; set; }
            public string destinationCode { get; set; }
            public string destinationName { get; set; }
            public int zoneCode { get; set; }
            public string zoneName { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public Room[] rooms { get; set; }
            public string minRate { get; set; }
            public string maxRate { get; set; }
            public string currency { get; set; }
        }

        public class Room
        {
            public string code { get; set; }
            public string name { get; set; }
            public Rate[] rates { get; set; }
        }

        public class Rate
        {
            public string rateKey { get; set; }
            public string rateClass { get; set; }
            public string rateType { get; set; }
            public string net { get; set; }
            public string sellingRate { get; set; }
            public bool hotelMandatory { get; set; }
            public int allotment { get; set; }
            public string rateCommentsId { get; set; }
            public string paymentType { get; set; }
            public bool packaging { get; set; }
            public string boardCode { get; set; }
            public string boardName { get; set; }
            public Cancellationpolicy[] cancellationPolicies { get; set; }
            public int rooms { get; set; }
            public int adults { get; set; }
            public int children { get; set; }
            public Offer[] offers { get; set; }
        }

        public class Cancellationpolicy
        {
            public string amount { get; set; }
            public DateTime from { get; set; }
        }

        public class Offer
        {
            public string code { get; set; }
            public string name { get; set; }
            public string amount { get; set; }
        }

    }
}

#nullable restore warnings
#pragma warning restore CA1819 // Properties should not return arrays
#pragma warning restore CA1034 // Nested types should not be visible
#pragma warning restore CA1707 // Identifiers should not contain underscores