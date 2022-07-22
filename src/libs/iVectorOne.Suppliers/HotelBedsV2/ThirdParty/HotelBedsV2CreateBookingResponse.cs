#nullable disable warnings
#pragma warning disable CA1819 // Properties should not return arrays
#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1707 // Identifiers should not contain underscores
#pragma warning disable CA1724 // Type names should not match namespaces

namespace iVectorOne.CSSuppliers.HotelBedsV2
{
    using System;

    public class HotelBedsV2CreateBookingResponse
    {
        public Auditdata auditData { get; set; }
        public Booking booking { get; set; }

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

        public class Booking
        {
            public string reference { get; set; }
            public string clientReference { get; set; }
            public string creationDate { get; set; }
            public string status { get; set; }
            public Modificationpolicies modificationPolicies { get; set; }
            public string creationUser { get; set; }
            public Holder holder { get; set; }
            public Hotel hotel { get; set; }
            public string remark { get; set; }
            public Invoicecompany invoiceCompany { get; set; }
            public float totalNet { get; set; }
            public float pendingAmount { get; set; }
            public string currency { get; set; }
        }

        public class Modificationpolicies
        {
            public bool cancellation { get; set; }
            public bool modification { get; set; }
        }

        public class Holder
        {
            public string name { get; set; }
            public string surname { get; set; }
        }

        public class Hotel
        {
            public string checkOut { get; set; }
            public string checkIn { get; set; }
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
            public string totalNet { get; set; }
            public string currency { get; set; }
            public Supplier supplier { get; set; }
        }

        public class Supplier
        {
            public string name { get; set; }
            public string vatNumber { get; set; }
        }

        public class Room
        {
            public string status { get; set; }
            public int id { get; set; }
            public string code { get; set; }
            public string name { get; set; }
            public string supplierReference { get; set; }
            public Pax[] paxes { get; set; }
            public Rate[] rates { get; set; }
        }

        public class Pax
        {
            public int roomId { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public string surname { get; set; }
        }

        public class Rate
        {
            public string rateClass { get; set; }
            public string net { get; set; }
            public string sellingRate { get; set; }
            public bool hotelMandatory { get; set; }
            public string rateComments { get; set; }
            public string paymentType { get; set; }
            public bool packaging { get; set; }
            public string boardCode { get; set; }
            public string boardName { get; set; }
            public Cancellationpolicy[] cancellationPolicies { get; set; }
            public Taxes taxes { get; set; }
            public int rooms { get; set; }
            public int adults { get; set; }
            public int children { get; set; }
        }

        public class Taxes
        {
            public Tax[] taxes { get; set; }
            public bool allIncluded { get; set; }
        }

        public class Tax
        {
            public bool included { get; set; }
            public string amount { get; set; }
            public string currency { get; set; }
        }

        public class Cancellationpolicy
        {
            public string amount { get; set; }
            public DateTime from { get; set; }
        }

        public class Invoicecompany
        {
            public string code { get; set; }
            public string company { get; set; }
            public string registrationNumber { get; set; }
        }

    }
}

#nullable restore warnings
#pragma warning restore CA1819 // Properties should not return arrays
#pragma warning restore CA1034 // Nested types should not be visible
#pragma warning restore CA1707 // Identifiers should not contain underscores
#pragma warning restore CA1724 // Type names should not match namespaces