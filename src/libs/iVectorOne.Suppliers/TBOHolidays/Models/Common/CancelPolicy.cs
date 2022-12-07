namespace iVectorOne.Suppliers.TBOHolidays.Models.Common
{
    using Newtonsoft.Json;

    public class CancelPolicy
    {
        [JsonProperty("Index")]
        public string Index { get; set; } = string.Empty;

        [JsonProperty("FromDate")]
        public string FromDate { get; set; } = string.Empty;

        [JsonProperty("ChargeType")]
        public ChargeType ChargeType { get; set; }

        [JsonProperty("CancellationCharge")]
        public decimal CancellationCharge { get; set; }
    }
}

