namespace iVectorOne.Suppliers.DerbySoft.Models
{
    using Newtonsoft.Json;

    public class ProductCandidate
    {
        [JsonProperty("roomId")] 
        public string RoomId { get; set; }

        [JsonProperty("rateId")]
        public string RateId { get; set; }
    }
}