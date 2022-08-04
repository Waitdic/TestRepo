namespace iVectorOne.Suppliers.ExpediaRapid.SerializableClasses.Book
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class RetrieveResponse : BookResponse, IExpediaRapidResponse<RetrieveResponse>
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("errors")]
        public List<Error> Errors { get; set; }

        public new (bool valid, RetrieveResponse response) GetValidResults(string responseString, int statusCode)
        {
            (bool valid, RetrieveResponse response) = (false, new RetrieveResponse());

            if (!string.IsNullOrWhiteSpace(responseString))
            {
                try
                {
                    response = JsonConvert.DeserializeObject<RetrieveResponse>(responseString)!;

                    if (!string.IsNullOrWhiteSpace(response.Type))
                    {
                        valid = true;
                    }
                }
                catch
                {
                }
            }

            return (valid, response);
        }
    }

    public class Error
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("fields")]
        public List<Field> Fields { get; set; }
    }

    public class Field
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
