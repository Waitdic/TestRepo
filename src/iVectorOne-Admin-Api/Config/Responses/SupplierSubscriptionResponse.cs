using iVectorOne_Admin_Api.Config.Models;

namespace iVectorOne_Admin_Api.Config.Requests
{
    public class SupplierSubscriptionResponse
    {
        public bool Success { get; set; }
        public SupplierSubscriptionDTO SupplierSubscription { get; set; } = new SupplierSubscriptionDTO();
        public List<string> Warnings { get; set; } = new List<string>();
    }
}
