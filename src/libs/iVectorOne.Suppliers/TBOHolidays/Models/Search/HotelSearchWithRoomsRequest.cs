namespace iVectorOne.Suppliers.TBOHolidays.Models.Search
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using iVectorOne.Suppliers.TBOHolidays.Models.Common;

    public class HotelSearchWithRoomsRequest
    {
        [JsonProperty("CheckIn")]
        public string CheckIn { get; set; } = string.Empty;

        [JsonProperty("CheckOut")]
        public string CheckOut { get; set; } = string.Empty;

        [JsonProperty("HotelCodes")]
        public string HotelCodes { get; set; } = string.Empty;

        [JsonProperty("GuestNationality")]
        public string GuestNationality { get; set; } = string.Empty;

        [JsonProperty("PaxRooms")]
        public List<PaxRoom> PaxRooms { get; set; } = new();

        [JsonProperty("ResponseTime")]
        public decimal ResponseTime { get; set; }

        [JsonProperty("IsDetailedResponse")]
        public bool IsDetailedResponse { get; set; }

        [JsonProperty("Filters")]
        public Filters Filters { get; set; } = new();
    }
}
