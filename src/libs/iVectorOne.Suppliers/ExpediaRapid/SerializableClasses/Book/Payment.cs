namespace iVectorOne.Suppliers.ExpediaRapid.SerializableClasses.Book
{
    using Newtonsoft.Json;
    public class Payment
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("billing_contact")]
        public BillingContact BillingContact { get; set; }

    }

}