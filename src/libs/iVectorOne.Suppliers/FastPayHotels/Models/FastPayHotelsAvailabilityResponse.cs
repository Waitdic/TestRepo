namespace iVectorOne.CSSuppliers.FastPayHotels.Models
{
    using System.Collections.Generic;

    public class FastPayHotelsAvailabilityResponse
    {
        public string messageID { get; set; } = string.Empty;
        public List<HotelAvail> hotelAvails { get; set; } = new List<HotelAvail>();

        public class HotelAvail
        {
            public HotelInfo hotelinfo { get; set; } = new HotelInfo();
            public List<AvailRoomRate> availRoomRates { get; set; } = new List<AvailRoomRate>();
        }

        public class HotelInfo
        {
            public string name { get; set; } = string.Empty;
            public string code { get; set; } = string.Empty;
        }

        public class AvailRoomRate
        {
            public string reservationToken { get; set; } = string.Empty;
            public string roomName { get; set; } = string.Empty;
            public SharedModels.Occupancy occupancy { get; set; } = new SharedModels.Occupancy();
            public string code { get; set; } = string.Empty;
            public string mealPlanCode { get; set; } = string.Empty;
            public string mealPlanName { get; set; } = string.Empty;
            public decimal totalPrice { get; set; }
            public bool priceBinding { get; set; }
            public decimal publicPrice { get; set; }
            public decimal commission { get; set; }
            public List<int> pricePerDay { get; set; } = new List<int>();
            public string currency { get; set; } = string.Empty;
            public CancellationPolicy cancellationPolicy = new CancellationPolicy();
            public string specialNotes { get; set; } = string.Empty;
        }

        public class CancellationPolicy
        {
            public string description { get; set; } = string.Empty;
            public string code { get; set; } = string.Empty;
            public bool cancellable { get; set; }
            public List<Penalty> penalties { get; set; } = new List<Penalty>();
        }

        public class Penalty
        {
            public Canceldeadline cancelDeadLine { get; set; } = new Canceldeadline();
            public PenaltyCharge penaltyCharge { get; set; } = new PenaltyCharge();
        }

        public class Canceldeadline
        {
            public bool noShow { get; set; }
            public string offsetTimeDropType { get; set; } = string.Empty;
            public string offsetTimeUnit { get; set; } = string.Empty;
            public int offsetTimeValue { get; set; }
        }

        public class PenaltyCharge
        {
            public string chargeBase { get; set; } = string.Empty;
            public int nights { get; set; }
            public int amount { get; set; }
            public int percent { get; set; }
        }
    }
}