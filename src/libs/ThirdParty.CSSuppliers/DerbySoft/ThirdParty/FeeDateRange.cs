﻿namespace ThirdPartyInterfaces.DerbySoft.ThirdParty
{
    using System;
    using Newtonsoft.Json;

    public class FeeDateRange
    {
        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }
    }
}