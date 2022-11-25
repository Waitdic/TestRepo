namespace iVectorOne.Suppliers.Polaris.Models
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class Notes 
    {
        [JsonPropertyName("warnings")]
        public List<Warn> Warnings { get; set; } = new();

        [JsonPropertyName("err")]
        public List<Err> Err { get; set; } = new();
    }

}
