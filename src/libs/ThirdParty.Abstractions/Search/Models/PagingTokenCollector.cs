namespace ThirdParty.Search.Models
{
    using System.Collections.Generic;
    using ThirdParty.Interfaces;

    public class PagingTokenCollector : IPagingTokenCollector
    {
        public int CurrentPage { get; set; }

        public int MaxPages { get; set; }

        public Dictionary<IPagingTokenKey, string> NextPageTokens { get; set; } = new();

        public PagingTokenCollector(int maxPages)
        {
            CurrentPage = 0;
            MaxPages = maxPages;
        }

        public void NextPage()
        {
            NextPageTokens.Clear();
            CurrentPage++;
        }
    }
}