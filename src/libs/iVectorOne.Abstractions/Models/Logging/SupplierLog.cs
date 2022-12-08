namespace iVectorOne.Models.Logging
{
    using Intuitive.Helpers.Net;

    public class SupplierLog
    {
        public string Title { get; set; } = string.Empty;
        public Request Request { get; set; } = null!;
    }
}