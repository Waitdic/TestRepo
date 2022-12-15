
namespace iVectorOne.SDK.V2
{
    using System.Collections.Generic;
    public record TransferRequestBase: RequestBase
    {
        public Dictionary<string, string> ThirdPartySettings { get; set; } = new Dictionary<string, string> { };

    }
}
