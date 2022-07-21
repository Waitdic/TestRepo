namespace ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses
{
    using Newtonsoft.Json;

    public class OccupancyRateFee
    {

        [JsonProperty("billable_currency")]
        public OccupancyRateAmount TotalInBillableCurrency { get; set; }

        [JsonProperty("request_currency")]
        public OccupancyRateAmount TotalInRequestCurrency { get; set; }

        [JsonProperty("scope")]
        public string FeeScope { get; set; }

        [JsonProperty("frequency")]
        public string FeeFrequency { get; set; }

    }

}