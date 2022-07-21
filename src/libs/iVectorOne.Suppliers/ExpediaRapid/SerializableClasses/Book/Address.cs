namespace ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses.Book
{
    using Newtonsoft.Json;

    public class Address
    {

        [JsonProperty("line_1")]
        public string Line1 { get; set; }

        [JsonProperty("line_2")]
        public string Line2 { get; set; }

        [JsonProperty("line_3")]
        public string Line3 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state_province_code")]
        public string StateProvinceCode { get; set; }

        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

    }

}