namespace ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses.Search
{
    using System;
    using Newtonsoft.Json;


    public class CancelPenalty
    {

        [JsonProperty("currency")]
        public string CurrencyCode { get; set; }

        [JsonProperty("start")]
        public DateTime CancelStartDate { get; set; }

        [JsonProperty("end")]
        public DateTime CancelEndDate { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("nights")]
        public int Nights { get; set; }

        [JsonProperty("percent")]
        public string Percent { get; set; }

    }

}