namespace ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses
{
    using Newtonsoft.Json;

    public class Link
    {

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("href")]
        public string HRef { get; set; }

    }

}