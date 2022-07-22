namespace iVectorOne.CSSuppliers.ExpediaRapid.SerializableClasses
{
    using System.Collections.Generic;
    using Newtonsoft.Json;


    public class OccupancyRoomRate
    {

        [JsonProperty("nightly")]
        public List<List<Rate>> NightlyRates { get; set; } = new List<List<Rate>>();

        [JsonProperty("stay")]
        public List<Rate> StayRates { get; set; } = new List<Rate>();

        [JsonProperty("totals")]
        public Dictionary<string, OccupancyRateTotal> OccupancyRateTotals { get; set; } = new Dictionary<string, OccupancyRateTotal>();

        [JsonProperty("fees")]
        public Dictionary<string, OccupancyRateFee> OccupancyRateFees { get; set; } = new Dictionary<string, OccupancyRateFee>();

    }

}