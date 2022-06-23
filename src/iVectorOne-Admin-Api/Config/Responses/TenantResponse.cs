using iVectorOne_Admin_Api.Config.Models;

namespace iVectorOne_Admin_Api.Config.Requests
{
    public class TenantResponse
    {
        public bool Success { get; set; }
        public List<SubscriptionDTO> Subscriptions { get; set; } = new List<SubscriptionDTO>();
        public List<string> Warnings { get; set; } = new List<string>();
    }
}
