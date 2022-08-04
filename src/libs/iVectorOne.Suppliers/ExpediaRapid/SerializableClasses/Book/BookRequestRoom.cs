namespace iVectorOne.Suppliers.ExpediaRapid.SerializableClasses.Book
{
    using Newtonsoft.Json;


    public class BookRequestRoom
    {

        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

        [JsonProperty("smoking")]
        public bool Smoking { get; set; }

        [JsonProperty("special_request")]
        public string SpecialRequest { get; set; }

    }

}