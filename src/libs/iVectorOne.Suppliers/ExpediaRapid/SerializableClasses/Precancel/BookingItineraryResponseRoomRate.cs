namespace iVectorOne.CSSuppliers.ExpediaRapid.SerializableClasses.BookingItinerary
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using iVectorOne.CSSuppliers.ExpediaRapid.SerializableClasses.Search;

    public class BookingItineraryResponseRoomRate
    {

        [JsonProperty("id")]
        public string RateID { get; set; }

        [JsonProperty("refundable")]
        public bool IsRefundable { get; set; }

        [JsonProperty("cancel_refund")]
        public CancelRefund CancelRefund { get; set; }

        [JsonProperty("merchant_of_record")]
        public string MerchantOfRecord { get; set; }

        [JsonProperty("amenities")]
        public List<string> Amenities { get; set; } = new List<string>();

        [JsonProperty("links")]
        public Dictionary<string, Link> RateLinks { get; set; } = new Dictionary<string, Link>();

        [JsonProperty("cancel_penalties")]
        public List<CancelPenalty> CancelPenalities { get; set; } = new List<CancelPenalty>();

        [JsonProperty("deposit_policies")]
        public List<DepositPolicy> DepositPolicies { get; set; } = new List<DepositPolicy>();

        [JsonProperty("nightly")]
        public List<List<Rate>> NightlyRates { get; set; } = new List<List<Rate>>();

        [JsonProperty("stay")]
        public List<Rate> StayRates { get; set; } = new List<Rate>();

        [JsonProperty("fees")]
        public List<Fee> OccupancyRateFees { get; set; } = new List<Fee>();

        [JsonProperty("promotions")]
        public Promotions Promotions { get; set; }

    }
}