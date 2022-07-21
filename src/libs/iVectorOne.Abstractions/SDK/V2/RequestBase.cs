namespace ThirdParty.SDK.V2
{
    using ThirdParty.Models;

    public record RequestBase
    {
        public Subscription User { get; set; } = new();
    }
}