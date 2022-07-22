namespace iVectorOne.SDK.V2
{
    using iVectorOne.Models;

    public record RequestBase
    {
        public Subscription User { get; set; } = new();
    }
}