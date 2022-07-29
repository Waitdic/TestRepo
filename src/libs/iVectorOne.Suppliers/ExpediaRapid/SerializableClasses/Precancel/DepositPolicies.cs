namespace iVectorOne.Suppliers.ExpediaRapid.SerializableClasses.BookingItinerary
{
    using System;
    using Newtonsoft.Json;


    public class DepositPolicy
    {

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("due")]
        public DateTime Due { get; set; }

    }

}