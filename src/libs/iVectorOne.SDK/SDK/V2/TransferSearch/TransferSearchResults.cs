namespace iVectorOne.SDK.V2.TransferSearch
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;
    public class TransferSearchResults
    {
        [XmlIgnore]
        public List<TransferSearchResult> SearchResults = new List<TransferSearchResult>();

        public List<TransferSearchResult> GetValidResults
        {
            get
            {
                return SearchResults.Where(x => x.Warnings.Count == 0).ToList();
            }
        }

    }
}
