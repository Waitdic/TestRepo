namespace iVectorOne.Suppliers.Polaris.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class AvailResponse
    {
        [JsonProperty("hotels")]
        public List<Hotel> Hotels { get; set; } = new();

        [JsonProperty("notes")]
        public Notes Notes { get; set; } = new();
    }

}
