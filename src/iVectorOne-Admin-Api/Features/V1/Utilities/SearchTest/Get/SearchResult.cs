namespace iVectorOne_Admin_Api.Features.V1.Utilities.SearchTest.Get
{
    public class SearchResult
    {
        public string Supplier { get; set; } = "";

        public string RoomCode { get; set; } = "";

        public string RoomType { get; set; } = "";

        public string MealBasis { get; set; } = "";

        public string Currency { get; set; } = "";

        public decimal TotalCost { get; set; }

        public bool NonRefundable { get; set; }
    }
}
