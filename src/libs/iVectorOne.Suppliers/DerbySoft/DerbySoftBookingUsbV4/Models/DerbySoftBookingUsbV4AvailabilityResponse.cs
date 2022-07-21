﻿namespace iVectorOne.CSSuppliers.DerbySoft.DerbySoftBookingUsbV4.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using iVectorOne.CSSuppliers.DerbySoft.Models;

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