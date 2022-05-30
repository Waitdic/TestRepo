namespace ThirdPartyInterfaces.DerbySoft.ThirdParty
{
    using System;
    using Newtonsoft.Json;

    public class StayRange
    {
        [JsonProperty("checkin")]
        public DateTime? CheckIn { get; set; }

        [JsonProperty("checkout")]
        public DateTime? CheckOut { get; set; }
    }
}