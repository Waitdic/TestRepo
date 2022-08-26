namespace iVectorOne.SDK.V2
{
    using iVectorOne.Models;

    public record RequestBase
    {
        public Account Account { get; set; } = new();
    }
}