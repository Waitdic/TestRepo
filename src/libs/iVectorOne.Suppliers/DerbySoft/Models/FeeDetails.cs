namespace iVectorOne.Suppliers.DerbySoft.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class FeeDetails
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FeeType? Type { get; set; }

        [JsonProperty("amount")]
        public decimal? Amount { get; set; }

        [JsonProperty("amountType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FeeAmountType? AmountType { get; set; }

        [JsonProperty("chargeType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ChargeType? ChargeType { get; set; }
    }
}