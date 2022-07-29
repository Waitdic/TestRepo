namespace iVectorOne.Suppliers.DerbySoft.Models
{
    using Newtonsoft.Json;

    public class Header
    {
        [JsonProperty("supplierId", NullValueHandling = NullValueHandling.Ignore)]
        public string SupplierId { get; set; }

        [JsonProperty("distributorId")]
        public string DistributorId { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}