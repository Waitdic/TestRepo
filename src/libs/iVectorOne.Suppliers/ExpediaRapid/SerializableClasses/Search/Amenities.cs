namespace ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses.Search
{
    using Newtonsoft.Json;

    public class Amenities
    {

        [JsonProperty("id")]
        public string AmenityID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

    }
}