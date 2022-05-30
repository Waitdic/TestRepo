
namespace ThirdParty.CSSuppliers.AceRooms.Models
{
using System.Collections.Generic;

    public class AceroomsBookRequest
    {
        public string SearchToken { get; set; } = string.Empty;
        public string ClientReference { get; set; } = string.Empty;
        public List<RoomDetails> Rooms { get; set; } = new List<RoomDetails>();

        public class RoomDetails
        {
            public int RoomNumber { get; set; }
            public string PreBookingToken { get; set; } = string.Empty;
            public string SpecialRequest { get; set; } = string.Empty;
            public List<Guest> Guests { get; set; } = new List<Guest>();
        }

        public class Guest
        {
            public string Title { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public int Age { get; set; }
        }
    }
}
