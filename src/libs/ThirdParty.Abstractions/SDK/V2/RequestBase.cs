namespace ThirdParty.SDK.V2
{
    using ThirdParty.Models;

    public record RequestBase
    {
        public User User { get; set; }
    }
}