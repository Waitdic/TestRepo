namespace iVectorOne.Suppliers.DerbySoft.Models
{
    using Newtonsoft.Json;

    public class RoomCriteria
    {
        [JsonProperty("roomCount")]
        public int RoomCount { get; set; }

        [JsonProperty("adultCount")]
        public int AdultCount { get; set; }

        [JsonProperty("childCount")]
        public int ChildCount { get; set; }

        [JsonProperty("childAges")]
        public int[] ChildAges { get; set; }
    }
}