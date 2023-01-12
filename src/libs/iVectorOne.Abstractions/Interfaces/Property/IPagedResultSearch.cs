namespace iVectorOne.Interfaces
{
    using iVectorOne.Search.Models;

    public interface IPagedResultSearch
    {
        int MaxPages(SearchDetails searchDetails);
    }
}