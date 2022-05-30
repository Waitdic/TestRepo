namespace ThirdPartyInterfaces.DerbySoft.DerbySoftShoppingEngineV4.ThirdParty
{
    using DerbySoft.ThirdParty;
    using Newtonsoft.Json;

    public class DerbySoftShoppingEngineV4SearchResponse
    {
        [JsonProperty("header")]
        public Header Header { get; set; }

        [JsonProperty("availHotels")]
        public AvailableHotel[] AvailableHotels { get; set; }
    }
}
