namespace iVectorOne_Admin_Api.Features.V1.Utilities.SearchTest
{
    public record ResponseModel : ResponseModelBase
    {
        public Guid RequestKey { get; set; }

        public string Message { get; set; } = string.Empty;

        //public List<SearchResult> Results { get; set; } = new List<SearchResult>();
    }
}
