namespace ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses.Search
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class SearchResponse : List<PropertyAvailablility>, IExpediaRapidResponse<SearchResponse>
    {
        public (bool valid, SearchResponse response) GetValidResults(string responseString, int statusCode)
        {
            (bool valid, SearchResponse response) = (false, new SearchResponse());
            if (!string.IsNullOrWhiteSpace(responseString))
            {
                try
                {
                    response = JsonConvert.DeserializeObject<SearchResponse>(responseString)!;
                    valid = true;
                }
                catch
                {
                }
            }
            return (valid, response);
        }
    }
}