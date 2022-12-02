namespace iVectorOne.Suppliers.TBOHolidays.Models.Common
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class HotelRoom
    {
        public List<string> Name { get; set; } = new();

        public string BookingCode { get; set; } = string.Empty;

        public string Inclusion { get; set; } = string.Empty;

        public decimal TotalFare { get; set; }

        public decimal TotalTax { get; set; }

        public List<string> RoomPromotion { get; set; }

        public string MealType { get; set; } = string.Empty;

        public bool IsRefundable { get; set; }

        public decimal RecommendedSellingRate { get; set; }

        [JsonProperty("Supplements")] 
        public List<Supplement[]> Supplements { get; set; } = new();

        public bool WithTransfers { get; set; }

        [JsonProperty("CancelPolicies")]
        public List<CancelPolicy> CancelPolicies { get; set; } = new();

        public int RoomIndex { get; set; }
    }
}
