namespace iVectorOne_Admin_Api.Adaptors.Search
{
    public record Response
    {
        public enum SearchStatusEnum { Ok, NotOk, NoResults, Exception };

        public SearchStatusEnum SearchStatus { get; set; }

        public iVectorOne.SDK.V2.PropertySearch.Response SearchResult { get; set; } = new iVectorOne.SDK.V2.PropertySearch.Response();

        public string Information { get; set; } = "";
    }
}
