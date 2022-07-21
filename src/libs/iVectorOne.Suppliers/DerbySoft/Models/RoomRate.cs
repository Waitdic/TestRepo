namespace iVectorOne.Suppliers.DerbySoft.Models
{
    using Newtonsoft.Json;

    public class RoomRate
    {
        [JsonProperty("roomCriteria", NullValueHandling = NullValueHandling.Ignore)]
        public RoomCriteria RoomCriteria { get; set; }

        [JsonProperty("inventory", NullValueHandling = NullValueHandling.Ignore)]
        public int? Inventory { get; set; }

        [JsonProperty("roomId")]
        public string RoomId { get; set; }

        [JsonProperty("rateId")]
        public string RateId { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("amountBeforeTax")]
        public decimal[] DailyCostBeforeTax { get; set; }

        [JsonProperty("amountAfterTax")]
        public decimal[] DailyCostAfterTax { get; set; }

        [JsonProperty("mealPlan")]
        public string MealPlan { get; set; }

        [JsonProperty("cancelPolicy", NullValueHandling = NullValueHandling.Ignore)]
        public CancelPolicy CancelPolicy { get; set; }

        [JsonProperty("fees", NullValueHandling = NullValueHandling.Ignore)]
        public Fee[] Fees { get; set; }
    }
}