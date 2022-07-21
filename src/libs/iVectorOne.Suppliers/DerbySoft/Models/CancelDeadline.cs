namespace ThirdParty.CSSuppliers.DerbySoft.Models
{
    using Newtonsoft.Json;

    public class CancelDeadline
    {
        [JsonProperty("offsetTimeDropType")]
        public OffsetTimeDropType OffsetTimeDropType { get; set; }

        [JsonProperty("offsetTimeUnit")]
        public OffsetTimeUnit OffsetTimeUnit { get; set; }

        [JsonProperty("offsetTimeValue")]
        public int OffsetTimeValue { get; set; }

        [JsonProperty("dealineTime")]
        public string DeadlineTime { get; set; }
    }
}