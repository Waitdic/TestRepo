namespace ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses.Prebook
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class PrebookResponse : IExpediaRapidResponse
    {

        [JsonProperty("occupancy_pricing")]
        public Dictionary<string, OccupancyRoomRate> OccupancyRoomRates { get; set; }

        [JsonProperty("links")]
        public Dictionary<string, Link> Links { get; set; } = new Dictionary<string, Link>();

        public bool IsValid(string responseString, int statusCode)
        {

            if (!string.IsNullOrWhiteSpace(responseString))
            {
                try
                {

                    JsonConvert.DeserializeObject<PrebookResponse>(responseString);
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