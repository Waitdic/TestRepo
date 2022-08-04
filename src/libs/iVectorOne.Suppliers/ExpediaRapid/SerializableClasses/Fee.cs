namespace iVectorOne.Suppliers.ExpediaRapid.SerializableClasses
{
    using Newtonsoft.Json;

    public class Fee
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public decimal Value { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

    }

}