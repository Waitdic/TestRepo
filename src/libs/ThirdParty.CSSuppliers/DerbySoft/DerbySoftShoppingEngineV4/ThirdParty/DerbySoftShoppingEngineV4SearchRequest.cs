namespace ThirdPartyInterfaces.DerbySoft.DerbySoftShoppingEngineV4.ThirdParty
{
    using DerbySoft.ThirdParty;
    using Newtonsoft.Json;

    public class DerbySoftShoppingEngineV4SearchRequest
    {
        [JsonProperty("header")]
        public Header Header { get; set; }

        [JsonProperty("hotels")]
        public Hotel[] Hotels { get; set; }

        [JsonProperty("stayRange")]
        public StayRange StayRange { get; set; }

        [JsonProperty("roomCriteria")]
        public RoomCriteria RoomCriteria { get; set; }
    }
}
