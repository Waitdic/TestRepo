
namespace iVectorOne.CSSuppliers.AceRooms.Models
{
using System.Collections.Generic;
using iVectorOne.CSSuppliers.AcerRooms.Models;

    /// <summary>
    /// The Acerooms response class to be deserialized
    /// </summary>
    public class AceroomsAvailabilityResponse
    {
        public AuditDataDetails AuditData { get; set; } = new AuditDataDetails();
        public SearchInfoDetails SearchInfo = new SearchInfoDetails();
        public List<HotelDetails> Hotels { get; set; } = new List<HotelDetails>();
        public int TotalHotels { get; set; }


        public class SearchInfoDetails
        {
            public bool CheapestRoom { get; set; }
            public string ArrivalDate { get; set; } = string.Empty;
            public int Nights { get; set; }
            public List<Room> Rooms { get; set; } = new List<Room>();
            public string CurrencyCode { get; set; } = string.Empty;
            public int SubAgentID { get; set; }
            public int CityID { get; set; }
            public string CityName { get; set; } = string.Empty;
            public int NationalityID { get; set; }
            public string Nationality { get; set; } = string.Empty;
        }

        public class Room
        {
            public int Adult { get; set; }
            public int Children { get; set; }
            public int RoomNumber { get; set; }
        }

        public class HotelDetails
        {
            public int HotelID { get; set; }
            public string Hotel { get; set; } = string.Empty;
            public decimal RatingID { get; set; }
            public string Rating { get; set; } = string.Empty;
            public List<RoomsDetails> Rooms { get; set; } = new List<RoomsDetails>();
        }

        public class RoomsDetails
        {
            public int RoomNumber { get; set; }
            public List<RateDetails> Rates { get; set; } = new List<RateDetails>();
        }

        public class RateDetails
        {
            public string RoomID { get; set; } = string.Empty;
            public string Room { get; set; } = string.Empty;
            public int BoardID { get; set; }
            public string Board { get; set; } = string.Empty;
            public decimal Rate { get; set; }
            public CancelPolicyDetails? CancelPolicy { get; set; }
        }

        public class CancelPolicyDetails
        {
            public string Description { get; set; } = string.Empty;
        }

    }
}