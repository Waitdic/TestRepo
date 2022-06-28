using iVectorOne_Admin_Api.Config.Models;

namespace iVectorOne_Admin_Api.Config.Responses
{
    public class SupplierSubscriptionResponse
    {
        public bool Success { get; set; }
        public int SubscriptionId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<SupplierDTO> SupplierSubscriptions { get; set; } = new List<SupplierDTO>();
        public List<string> Warnings { get; set; } = new List<string>();
    }
}
