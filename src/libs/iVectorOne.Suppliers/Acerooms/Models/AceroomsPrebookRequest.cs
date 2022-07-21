
namespace iVectorOne.Suppliers.AceRooms.Models
{
using System.Collections.Generic;

    /// <summary>
    /// The Acerooms prebook request class to be serialized and set as request string
    /// </summary>
    public class AceroomsPrebookRequest
    {
        public string SearchToken { get; set; } = string.Empty;
        public List<Room> Rooms { get; set; } = new List<Room>();

        public class Room
        {
            public int RoomNumber { get; set; }
            public string RoomID { get; set; } = string.Empty;
        }
    }
}
