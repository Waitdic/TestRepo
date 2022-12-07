namespace iVectorOne.Suppliers.TBOHolidays.Models.Common
{
    using Newtonsoft.Json;

    public class Filters
    {
        [JsonProperty("Refundable")]
        public bool Refundable { get; set; }

        [JsonProperty("NoOfRooms")]
        public int NoOfRooms { get; set; }

        [JsonProperty("MealType")]
        public string MealType { get; set; }
    }
}
