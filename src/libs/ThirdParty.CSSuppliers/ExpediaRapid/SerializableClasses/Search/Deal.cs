namespace ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses.Search
{
    using Newtonsoft.Json;

    public class Promotion
    {

        [JsonProperty("id")]
        public string PromotionID { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

    }

}