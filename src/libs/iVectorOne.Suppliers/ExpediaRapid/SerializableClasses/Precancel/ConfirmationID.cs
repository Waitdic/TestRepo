namespace iVectorOne.Suppliers.ExpediaRapid.SerializableClasses.BookingItinerary
{
    using Newtonsoft.Json;

    public class ConfirmationID
    {

        [JsonProperty("expedia")]
        public string ExpediaConfirmationID { get; set; }

        [JsonProperty("property")]
        public string PropertyConfirmationID { get; set; }

    }

}