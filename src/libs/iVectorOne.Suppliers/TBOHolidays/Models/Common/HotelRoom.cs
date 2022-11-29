using System;
using Newtonsoft.Json;

namespace iVectorOne.Suppliers.TBOHolidays.Models.Common
{
    using System.Collections.Generic;

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

        public int RoomIndex { get; set; }



        //public string RoomTypeCode { get; set; } = string.Empty;

        //public string RatePlanCode { get; set; } = string.Empty;

        //public RoomRate RoomRate { get; set; } = new();

        //public decimal Discount { get; set; }
    }
}
