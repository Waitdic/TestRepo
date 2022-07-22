#nullable disable warnings
#pragma warning disable CA1819 // Properties should not return arrays
#pragma warning disable CA1034 // Nested types should not be visible

namespace iVectorOne.CSSuppliers.HotelBedsV2
{
    public class HotelBedsV2AvailabilityRequest
    {
        public Stay stay { get; set; }
        public Occupancy[] occupancies { get; set; }
        public Hotels hotels { get; set; }
        public Rooms rooms { get; set; }
        public Keywords keywords { get; set; }
        public string[] accommodations { get; set; }
        public Boards boards { get; set; }
        public Review[] reviews { get; set; }
        public Filter filter { get; set; }
        public bool dailyRate { get; set; }
        public string sourceMarket { get; set; }
        public Destination destination { get; set; }
        public GeoLocation geoLocation { get; set; } 

        public class Stay
        {
            public string checkIn { get; set; }
            public string checkOut { get; set; }
            public string shiftDays { get; set; }
        }

        public class GeoLocation
        {
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
            public decimal radius { get; set; }
            public string unit { get; set; } = "km";
        }

        public class Destination
        {
            public string code { get; set; }
        }

        public class Hotels
        {
            public int[] hotel { get; set; }
        }

        public class Rooms
        {
            public bool included { get; set; }
            public string[] room { get; set; }
        }

        public class Keywords
        {
            public int[] keyword { get; set; }
        }

        public class Boards
        {
            public bool included { get; set; }
            public string[] board { get; set; }
        }

        public class Filter
        {
            public int minRate { get; set; }
            public int maxRate { get; set; }
            public int minCategory { get; set; }
            public int maxCategory { get; set; }
            public string paymentType { get; set; }
            public int maxRatesPerRoom { get; set; }
            public bool packaging { get; set; }
            public string hotelPackage { get; set; }
        }

        public class Occupancy
        {
            public int rooms { get; set; }
            public int adults { get; set; }
            public int children { get; set; }
            public Pax[] paxes { get; set; }
        }

        public class Pax
        {
            public string type { get; set; }
            public int age { get; set; }
        }

        public class Review
        {
            public string type { get; set; }
            public int maxRate { get; set; }
            public int minReviewCount { get; set; }
        }
    }
}

#nullable restore warnings
#pragma warning restore CA1819 // Properties should not return arrays
#pragma warning restore CA1034 // Nested types should not be visible