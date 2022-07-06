namespace ThirdParty.CSSuppliers.FastPayHotels.Models
{
    using System.Collections.Generic;

    class FastPayHotelsPrebookRequest
    {
        public string messageID { get; set; } = string.Empty;
        public List<Room> rooms { get; set; } = new List<Room>();

        public class Room
        {
            public string availToken { get; set; } = string.Empty;
            public int quantity { get; set; }

        }
    }
}