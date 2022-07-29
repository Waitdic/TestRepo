namespace iVectorOne.Suppliers.ExpediaRapid.SerializableClasses.Search
{
    using Newtonsoft.Json;


    public class BedConfiguration
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }

}