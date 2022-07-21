﻿namespace iVectorOne.CSSuppliers.DerbySoft.Models
{
    using Newtonsoft.Json;

    public class Fee
    {
        [JsonProperty("dateRange")]
        public FeeDateRange DateRange { get; set; }

        [JsonProperty("fee")]
        public FeeDetails FeeDetails { get; set; }
    }
}