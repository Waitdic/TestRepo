namespace ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses.Search
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class SearchResponse : List<PropertyAvailablility>, IExpediaRapidResponse
    {

        public bool IsValid(string responseString, int statusCode)
        {

            if (!string.IsNullOrWhiteSpace(responseString))
            {
                var token = JToken.Parse(responseString);
                try
                {

                    JsonConvert.DeserializeObject<SearchResponse>(responseString);
                    return true;
                }

                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }
    }

}