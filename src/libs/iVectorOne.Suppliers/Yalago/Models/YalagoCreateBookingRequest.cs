namespace iVectorOne.Suppliers.Models.Yalago
{
#pragma warning disable CS8618

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    class YalagoCreateBookingRequest
    {
        public string AffiliateRef { get; set; }
        public string CheckInDate { get; set; }
        public string CheckOutDate { get; set; }
        public int EstablishmentId { get; set; }
        public Room[] Rooms { get; set; }
        public string Culture { get; set; }
        [JsonProperty("ContactDetails")]
        public ContactDetails contactDetails { get; set; }
        public bool GetPackagePrice { get; set; }
        public string SourceMarket { get; set; }
        public bool GetTaxBreakdown { get; set; }
        public bool GetLocalCharges { get; set; }
        public bool GetErrataCategory { get; set; }
        public bool GetBoardBasis { get; set; }

        public class Room
        {
            public Guest[] Guests { get; set; }
            public string RoomCode { get; set; }
            public string BoardCode { get; set; }
            public ExpectedNetCost ExpectedNetCost { get; set; }
            public string AffiliateRoomRef { get; set; }
            public string SpecialRequests { get; set; }

        }
        public class ContactDetails
        {
            public string Title { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string PostCode { get; set; }
        }
        public class Guest
        {
            public string Title { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }
        }
        public class ExpectedNetCost
        {
            public decimal Amount { get; set; }
            public string Currency { get; set; }
        }
    }
}