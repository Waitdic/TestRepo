namespace iVectorOne_Admin_Api.Config.Responses
{
    using iVectorOne_Admin_Api.Config.Models;

    public class SupplierListResponse
    {
        public bool Success { get; set; }
        public List<string> Warnings { get; set; } = new();
        public List<SupplierListItemDTO> Suppliers { get; set; } = new();
    }
}