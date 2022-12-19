namespace iVectorOne.Suppliers.TBOHolidays.Models.Prebook
{
    using System.Collections.Generic;
    using Common;
    using Newtonsoft.Json;

    public class PrebookResponse
    {
        [JsonProperty("Status")]
        public Status Status { get; set; } = new();

        [JsonProperty("HotelResult")]
        public List<HotelResult> HotelResult { get; set; } = new();
    }
}