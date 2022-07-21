namespace ThirdParty.CSSuppliers.HotelsProV2.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ApiResonse<T> where T : class
    {
        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;

        [JsonProperty("results")]
        public List<T> Results { get; set; } = new();
    }
}
