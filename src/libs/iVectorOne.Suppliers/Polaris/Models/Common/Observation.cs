namespace iVectorOne.Suppliers.Polaris.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class Observation
    {
        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("txt")]
        public string Txt { get; set; } = string.Empty;
    }
}
