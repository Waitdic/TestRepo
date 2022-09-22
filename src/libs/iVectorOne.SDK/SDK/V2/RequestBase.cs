namespace iVectorOne.SDK.V2
{
    using iVectorOne.Models;
    using System.Text.Json.Serialization;

    public record RequestBase
    {
        [JsonIgnore]
        public Account Account { get; set; } = new();
    }
}