namespace iVectorOne.CSSuppliers.DerbySoft.DerbySoftShoppingEngineV4.Models
{
    using Newtonsoft.Json;
    using iVectorOne.CSSuppliers.DerbySoft.Models;

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