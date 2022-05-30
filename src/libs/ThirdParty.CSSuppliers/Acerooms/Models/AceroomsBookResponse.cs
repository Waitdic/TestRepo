
namespace ThirdParty.CSSuppliers.AceRooms.Models
{
using System.Collections.Generic;

    public class AceroomsBookResponse
    {
        public BookingDetails Booking { get; set; } = new BookingDetails();

        public string ErrorInfo { get; set; } = string.Empty;

        public class BookingDetails
        {
            public List<Room> Rooms { get; set; } = new List<Room>();
        }

        public class Room
        {
            public string BookingRoomID { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
        }
    }
}
