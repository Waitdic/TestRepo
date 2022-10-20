namespace iVectorOne_Admin_Api.Features.V1.Utilities.SearchTest
{
    public record SearchRequest
    {
        public DateTime ArrivalDate { get; set; }

        public int Duration { get; set; }

        public List<int> Properties { get; set; } = new();

        public List<RoomRequest> RoomRequests { get; set; } = new();

        //public string NationalityID { get; set; } = "";

        //public string CurrencyCode { get; set; } = "";

        //public bool OpaqueRates { get; set; }

        //public string SellingCountry { get; set; } = "";

        //public List<string> Suppliers { get; set; } = new();

        //public string EmailLogsToAddress { get; set; } = string.Empty;

        //public DedupeMethod DedupeMethod { get; set; } = DedupeMethod.cheapestleadin;
    }

    public class RoomRequest
    {
        public int Adults { get; set; }

        public int Children { get; set; }

        public int Infants { get; set; }

        public List<int> ChildAges { get; set; } = new List<int>();
    }
}
