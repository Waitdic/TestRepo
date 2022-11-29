namespace iVectorOne.Suppliers.Polaris.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Notes
    {
        [JsonProperty("warnings")]
        public List<Warn> Warnings { get; set; } = new();

        [JsonProperty("err")]
        public List<Err> Err { get; set; } = new();
    }

}
