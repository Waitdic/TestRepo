namespace iVectorOne.Suppliers.ExpediaRapid.SerializableClasses.Search
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class RoomRate
    {

        [JsonProperty("id")]
        public string RateID { get; set; }

        [JsonProperty("available_rooms")]
        public int AvailableRooms { get; set; }

        [JsonProperty("refundable")]
        public bool IsRefundable { get; set; }

        [JsonProperty("deposit_required")]
        public bool DepositRequired { get; set; }

        [JsonProperty("merchant_of_record")]
        public string MerchantOfRecord { get; set; }

        [JsonProperty("amenities")]
        public Dictionary<string, Amenities> Amenities { get; set; } = new Dictionary<string, Amenities>();

        [JsonProperty("links")]
        public Dictionary<string, Link> RateLinks { get; set; } = new Dictionary<string, Link>();

        [JsonProperty("bed_groups")]
        public Dictionary<string, BedGroupAvailability> BedGroupAvailabilities { get; set; } = new Dictionary<string, BedGroupAvailability>();

        [JsonProperty("cancel_penalties")]
        public List<CancelPenalty> CancelPenalities { get; set; } = new List<CancelPenalty>();

        [JsonProperty("occupancy_pricing")]
        public Dictionary<string, OccupancyRoomRate> OccupancyRoomRates { get; set; } = new Dictionary<string, OccupancyRoomRate>();

        [JsonProperty("promotions")]
        public Promotions Promotions { get; set; }

    }

}