namespace iVectorOne.SDK.V2
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public record ResponseBase
    {
        /// <summary>Any warnings raised on the cancellation response</summary>
        [JsonIgnore]
        public List<string> Warnings { get; set; } = new();
    }
}