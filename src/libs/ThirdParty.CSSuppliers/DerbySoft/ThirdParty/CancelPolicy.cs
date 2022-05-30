﻿namespace ThirdPartyInterfaces.DerbySoft.ThirdParty
{
    using Newtonsoft.Json;

    public class CancelPolicy
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("cancelPenalties")]
        public CancelPenalty[] CancelPenalties { get; set; }
    }
}