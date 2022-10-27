namespace iVectorOne_Admin_Api.Data.Models
{
    public class FireForgetSearchResponse
    {
        public int FireForgetSearchResponseId { get; set; }

        public string FireForgetSearchResponseKey { get; set; } = string.Empty;

        public string Information { get; set; } = string.Empty;

        public string SearchStatus { get; set; } = string.Empty;

        public iVectorOne.SDK.V2.PropertySearch.Response SearchResponse { get; set; } = new iVectorOne.SDK.V2.PropertySearch.Response();
    }
}
