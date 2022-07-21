namespace iVectorOne.Suppliers.HotelsProV2.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Product
    {
        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;

        [JsonProperty("nonrefundable")]
        public bool NonRefundable { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; } = string.Empty;

        [JsonProperty("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonProperty("meal_type")]
        public string MealType { get; set; } = string.Empty;

        [JsonProperty("rooms")]
        public List<Room> Rooms { get; set; } = new();
    }
}
