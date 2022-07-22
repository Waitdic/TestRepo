namespace iVectorOne.CSSuppliers.HotelsProV2.Models
{
    using Newtonsoft.Json;

    public class Policy
    {
        [JsonProperty("ratio")]
        public string Ratio { get; set; }

        [JsonProperty("days_remaining")]
        public string DaysRemaining { get; set; }
    }
}
