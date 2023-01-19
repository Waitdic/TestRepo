namespace iVectorOne.SDK.V2.ExtraSearch
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;
    public class ExtraSearchResults
    {
        [XmlIgnore]
        public List<ExtraSearchResult> SearchResults = new List<ExtraSearchResult>();

        public List<ExtraSearchResult> GetValidResults
        {
            get
            {
                return SearchResults.Where(x => x.Warnings.Count == 0).ToList();
            }
        }

    }
}
