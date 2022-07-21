namespace ThirdParty.CSSuppliers.HotelsProV2.Models
{
    using Newtonsoft.Json;

    public class CancellationResponse
    {
        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;

        [JsonProperty("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonProperty("charge_amount")]
        public string ChargeAmount { get; set; } = string.Empty;
    }
}
