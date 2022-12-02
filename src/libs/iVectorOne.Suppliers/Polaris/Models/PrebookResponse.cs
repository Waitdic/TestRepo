namespace iVectorOne.Suppliers.Polaris.Models
{
    public class PrebookResponse
    {
        public Hotel Hotel { get; set; } = new();
        public string Token { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int TTL { get; set; }
    }
}
