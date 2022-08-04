namespace iVectorOne.Suppliers.ExpediaRapid.SerializableClasses
{
    using Newtonsoft.Json;

    public class OccupancyRateFee
    {

        [JsonProperty("billable_currency")]
        public OccupancyRateAmount TotalInBillableCurrency { get; set; }

        [JsonProperty("request_currency")]
        public OccupancyRateAmount TotalInRequestCurrency { get; set; }

    }

}