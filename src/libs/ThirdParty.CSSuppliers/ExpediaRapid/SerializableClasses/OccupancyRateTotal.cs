namespace ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses
{
    using Newtonsoft.Json;

    public class OccupancyRateTotal
    {

        [JsonProperty("billable_currency")]
        public OccupancyRateAmount TotalInBillableCurrency { get; set; }

        [JsonProperty("request_currency")]
        public OccupancyRateAmount TotalInRequestCurrency { get; set; }

    }
}