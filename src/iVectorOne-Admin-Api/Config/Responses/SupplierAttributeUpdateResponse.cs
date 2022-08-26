namespace iVectorOne_Admin_Api.Config.Responses
{
    public class SupplierAttributeUpdateResponse
    {
        public bool Success { get; set; }
        public List<string> Warnings { get; set; } = new();
    }
}