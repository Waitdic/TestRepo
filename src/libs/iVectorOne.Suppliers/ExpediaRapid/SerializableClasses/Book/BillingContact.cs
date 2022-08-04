namespace iVectorOne.Suppliers.ExpediaRapid.SerializableClasses.Book
{
    using Newtonsoft.Json;


    public class BillingContact
    {

        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

        [JsonProperty("address")]
        public Address Address { get; set; }
    }

}