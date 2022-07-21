namespace iVectorOne.Suppliers.ExpediaRapid.SerializableClasses.Prebook
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class PrebookResponse : IExpediaRapidResponse<PrebookResponse>
    {
        [JsonProperty("occupancy_pricing")]
        public Dictionary<string, OccupancyRoomRate> OccupancyRoomRates { get; set; }

        [JsonProperty("links")]
        public Dictionary<string, Link> Links { get; set; } = new Dictionary<string, Link>();

        public (bool valid, PrebookResponse response) GetValidResults(string responseString, int statusCode)
        {
            (bool valid, PrebookResponse response) = (false, new PrebookResponse());
            if (!string.IsNullOrWhiteSpace(responseString))
            {
                try
                {
                    response = JsonConvert.DeserializeObject<PrebookResponse>(responseString)!;
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