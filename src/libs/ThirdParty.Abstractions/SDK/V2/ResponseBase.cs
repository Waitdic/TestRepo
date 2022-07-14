namespace ThirdParty.SDK.V2
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public record ResponseBase
    {
        /// <summary>Any warnings raised on the cancellation response</summary>
        /// <value>The warnings.</value>
        [JsonIgnore]
        public List<string> Warnings { get; set; } = new();
    }
}