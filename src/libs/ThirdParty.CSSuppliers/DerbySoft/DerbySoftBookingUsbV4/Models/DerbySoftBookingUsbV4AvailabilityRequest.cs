﻿namespace ThirdParty.CSSuppliers.DerbySoft.DerbySoftBookingUsbV4.Models
{
    using Newtonsoft.Json;
    using ThirdParty.CSSuppliers.DerbySoft.Models;

    public class DerbySoftBookingUsbV4AvailabilityRequest
    {
        [JsonProperty("header")]
        public Header Header { get; set; }

        [JsonProperty("hotelId")]
        public string HotelId { get; set; }

        [JsonProperty("stayRange")]
        public StayRange StayRange { get; set; }

        [JsonProperty("roomCriteria")]
        public RoomCriteria RoomCriteria { get; set; }

        [JsonProperty("productCandidate", NullValueHandling = NullValueHandling.Ignore)]
        public ProductCandidate ProductCandidate { get; set; }
    }
}