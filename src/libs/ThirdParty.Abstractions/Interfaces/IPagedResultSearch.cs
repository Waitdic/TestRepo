namespace ThirdParty.Interfaces
{
    using ThirdParty.Search.Models;

    public interface IPagedResultSearch
    {
        int MaxPages(SearchDetails searchDetails);
    }
}