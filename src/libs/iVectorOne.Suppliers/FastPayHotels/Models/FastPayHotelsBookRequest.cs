namespace ThirdParty.CSSuppliers.FastPayHotels.Models
{
    using System.Collections.Generic;

    public class FastPayHotelsBookRequest
    {
        public string messageID { get; set; } = string.Empty;
        public string currency { get; set; } = string.Empty;
        public string agencyCode { get; set; } = string.Empty;
        public string comments { get; set; } = string.Empty;
        public Customer customer { get; set; } = new Customer();
        public List<Room> rooms { get; set; } = new List<Room>();

        public class Customer
        {
            public string firstName { get; set; } = string.Empty;
            public string lastName { get; set; } = string.Empty;
            public string email { get; set; } = string.Empty;
            public string phone { get; set; } = string.Empty;
        }

        public class Room
        {
            public int adults { get; set; }
            public int children { get; set; }
            public List<Pax> paxes { get; set; } = new List<Pax>();
            public List<string> comments { get; set; } = new List<string>();
            public string reservationToken { get; set; } = string.Empty;
        }

        public class Pax
        {
            public string firstName { get; set; } = string.Empty;
            public string lastName { get; set; } = string.Empty;
            public int age { get; set; }
        }
    }
}