namespace iVectorOne.CSSuppliers.HotelsProV2.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ProvisionResponse
    {
        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;

        [JsonProperty("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonProperty("price")]
        public string Price { get; set; } = string.Empty;

        [JsonProperty("rooms")]
        public List<Room> Rooms { get; set; } = new();

        [JsonProperty("additional_info")]
        public string AdditionalInfo { get; set; } = string.Empty;

        [JsonProperty("policies")]
        public List<Policy> Policies { get; set; } = new();
    }
}
