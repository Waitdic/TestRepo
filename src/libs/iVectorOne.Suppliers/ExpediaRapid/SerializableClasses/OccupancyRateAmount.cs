namespace iVectorOne.Suppliers.ExpediaRapid.SerializableClasses
{
    using Newtonsoft.Json;

    public class OccupancyRateAmount
    {

        [JsonProperty("value")]
        public decimal Amount { get; set; }

        [JsonProperty("currency")]
        public string CurrencyCode { get; set; }

    }

}