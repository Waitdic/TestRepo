namespace iVectorOne_Admin_Api.Features.V1.Utilities.SearchTest
{
    public record ResponseModel : ResponseModelBase
    {
        public List<SearchResult> Results { get; set; }
    }
}
