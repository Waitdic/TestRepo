namespace ThirdPartyInterfaces.DerbySoft.DerbySoftBookingUsbV4.ThirdParty
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using DerbySoft.ThirdParty;

    public class DerbySoftBookingUsbV4AvailabilityResponse
    {
        [JsonProperty("header")]
        public Header Header { get; set; }

        [JsonProperty("hotelId")]
        public string HotelId { get; set; }

        [JsonProperty("roomRates")]
        public List<RoomRate> RoomRates { get; set; }
        
    }
}
