namespace ThirdParty.Interfaces
{
    using System.Collections.Generic;

    public interface IPagingTokenCollector
    {
        int CurrentPage { get; set; }

        int MaxPages { get; set; }

        Dictionary<IPagingTokenKey, string> NextPageTokens { get; set; }

        void NextPage();
    }
}