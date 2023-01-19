
namespace iVectorOne.SDK.V2
{
    using System.Collections.Generic;
    public record ComponentRequestBase: RequestBase
    {
        public Dictionary<string, string> ThirdPartySettings { get; set; } = new Dictionary<string, string> { };

    }
}
