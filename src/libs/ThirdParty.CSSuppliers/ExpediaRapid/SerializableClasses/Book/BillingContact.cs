namespace ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses.Book
{
    using Newtonsoft.Json;


    public class BillingContact
    {

        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public Phone Phone { get; set; }

        [JsonProperty("address")]
        public Address Address { get; set; }
    }

}