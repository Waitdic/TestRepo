namespace iVectorOne.Suppliers.TBOHolidays.Models.Common
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class HotelResponse
    {
        [JsonProperty("Status")]
        public Status Status { get; set; } = new();

        [JsonProperty("HotelResult")]
        public List<HotelResult> HotelResult { get; set; } = new();
    }
}
