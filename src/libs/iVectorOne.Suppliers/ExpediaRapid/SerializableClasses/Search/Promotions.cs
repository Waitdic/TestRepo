namespace ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses.Search
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Promotions
    {

        [JsonProperty("value_adds")]
        public Dictionary<string, Promotion> ValueAdds { get; set; } = new Dictionary<string, Promotion>();

        [JsonProperty("deal")]
        public Promotion Deal { get; set; }

    }

}