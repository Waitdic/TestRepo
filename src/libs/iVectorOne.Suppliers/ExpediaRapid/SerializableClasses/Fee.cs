namespace ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses
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

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("frequency")]
        public string Frequency { get; set; }

    }

}