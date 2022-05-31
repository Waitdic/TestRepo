namespace ThirdParty.CSSuppliers.DerbySoft.Models
{
    using Newtonsoft.Json;

    public class Hotel
    {
        [JsonProperty("supplierId")]
        public string SupplierId { get; set; }

        [JsonProperty("hotelId")]
        public string HotelId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}