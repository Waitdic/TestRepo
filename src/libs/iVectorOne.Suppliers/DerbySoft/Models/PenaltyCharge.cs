namespace iVectorOne.CSSuppliers.DerbySoft.Models
{
    using Newtonsoft.Json;

    public class PenaltyCharge
    {
        [JsonProperty("chargeBase")]
        public CancelChargeBase ChargeBase { get; set; }

        [JsonProperty("nights")]
        public int? Nights { get; set; }

        [JsonProperty("percent")]
        public decimal? Percent { get; set; }

        [JsonProperty("amount")]
        public decimal? Amount { get; set; }
    }
}