namespace iVectorOne.CSSuppliers.AceRooms.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// The Acerooms search request class to be serialized and set as request string
    /// </summary>
    public class AceroomsAvailabilityRequest
    {
        public int CityID { get; set; }
        public string NationalityID { get; set; } = string.Empty;
        public string ArrivalDate { get; set; } = string.Empty;
        public int Nights { get; set; }
        public List<int> Hotels { get; set; } = new List<int>();
        public string CurrencyCode { get; set; } = string.Empty;
        public List<Room> Rooms { get; set; } = new List<Room>();

        public class Room
        {
            public int Adult { get; set; }
            public int Children { get; set; }
            public int[]? Ages { get; set; }
        }
    }
}