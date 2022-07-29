namespace iVectorOne.Suppliers.ExpediaRapid.SerializableClasses.BookingItinerary
{
    using Newtonsoft.Json;

    public class CancelRefund
    {

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("currency")]
        public string CurrencyCode { get; set; }

    }

}