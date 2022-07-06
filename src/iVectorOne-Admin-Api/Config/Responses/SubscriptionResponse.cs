using iVectorOne_Admin_Api.Config.Models;

namespace iVectorOne_Admin_Api.Config.Responses
{
    public class SubscriptionResponse
    {
        public bool Success { get; set; }
        public SubscriptionDTO Subscription { get; set; } = new SubscriptionDTO();
        public List<string> Warnings { get; set; } = new List<string>();
    }
}
