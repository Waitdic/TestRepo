
namespace iVectorOne.CSSuppliers.AceRooms.Models
{
using System.Collections.Generic;
using iVectorOne.CSSuppliers.AcerRooms.Models;

    public class AceroomsPrebookResponse
    {
        public AuditDataDetails? AuditData { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public List<HotelDetails> Hotels { get; set; } = new List<HotelDetails>();

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
            public RateDetails Rate { get; set; } = new RateDetails();
        }

        public class RateDetails
        {
            public string PreBookingToken { get; set; } = string.Empty;
            public string Room { get; set; } = string.Empty;
            public int BoardID { get; set; }
            public string Board { get; set; } = string.Empty;
            public decimal Rate { get; set; }
            public List<CancelPolicyDetails> CancelPolicies { get; set; } = new List<CancelPolicyDetails>();
            public string Remarks { get; set; } = string.Empty;
        }

        public class CancelPolicyDetails
        {
            public string CancelDate { get; set; } = string.Empty;
            public decimal Amount { get; set; }
        }
    }
}
