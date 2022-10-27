namespace iVectorOne_Admin_Api.Data.Models
{
    public class FireForgetSearchResponse
    {
        public int FireForgetSearchResponseId { get; set; }

        //public string SearchKey { get; set; } = string.Empty;

        //public string Message { get; set; } = string.Empty;

        public iVectorOne.SDK.V2.PropertySearch.Response SearchResponse { get; set; } = new iVectorOne.SDK.V2.PropertySearch.Response();
    }
}
