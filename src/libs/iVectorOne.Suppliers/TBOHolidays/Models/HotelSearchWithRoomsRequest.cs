using iVectorOne.Suppliers.TBOHolidays.Models.Common;

namespace iVectorOne.Suppliers.TBOHolidays.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

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


        //public string CountryName { get; set; } = string.Empty;
        //public string CityName { get; set; } = string.Empty;
        //public string CityId { get; set; } = string.Empty;
        //public string IsNearBySearchAllowed { get; set; } = string.Empty;
        //public int NoOfRooms { get; set; }


        //public string PreferredCurrencyCode { get; set; } = string.Empty;
        //public string ResultCount { get; set; } = string.Empty;
    }
}
