namespace iVectorOne.CSSuppliers.DerbySoft.DerbySoftShoppingEngineV4.Models
{
    using Newtonsoft.Json;
    using iVectorOne.CSSuppliers.DerbySoft.Models;

    public class DerbySoftShoppingEngineV4SearchResponse
    {
        [JsonProperty("header")]
        public Header Header { get; set; }

        [JsonProperty("availHotels")]
        public AvailableHotel[] AvailableHotels { get; set; }
    }
}