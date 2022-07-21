namespace iVectorOne.CSSuppliers.ExpediaRapid.SerializableClasses.Search
{
    using Newtonsoft.Json;

    public class CancelRefund
    {

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("currency")]
        public string CurrencyCode { get; set; }

    }
}