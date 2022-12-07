namespace iVectorOne.Suppliers.TBOHolidays.Models.Common
{
    using Newtonsoft.Json;

    public class Supplement
    {
        [JsonProperty("Index")]
        public int Index { get; set; }

        [JsonProperty("Type")]
        public SuppChargeType Type { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("Price")]
        public decimal Price { get; set; }

        [JsonProperty("Currency")]
        public string Currency { get; set; } = string.Empty;
    }
}
