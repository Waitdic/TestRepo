namespace iVectorOne.Suppliers.ExpediaRapid.SerializableClasses
{
    using Newtonsoft.Json;

    public class Rate
    {


        [JsonProperty("type")]
        public string RateType { get; set; }

        [JsonProperty("value")]
        public decimal Amount { get; set; }

        [JsonProperty("currency")]
        public string CurrencyCode { get; set; }


    }
}