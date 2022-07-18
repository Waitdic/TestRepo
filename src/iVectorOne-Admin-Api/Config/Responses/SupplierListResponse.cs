using iVectorOne_Admin_Api.Config.Models;

namespace iVectorOne_Admin_Api.Config.Responses
{
    public class SupplierListResponse
    {
        public bool Success { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
        public List<SupplierListItemDTO> Suppliers { get; set; } = new List<SupplierListItemDTO>();
    }
}