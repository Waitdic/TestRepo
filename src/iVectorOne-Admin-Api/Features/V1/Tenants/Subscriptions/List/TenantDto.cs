using iVectorOne_Admin_Api.Config.Models;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.List
{
    public record TenantDto
    {
        public string TenantName { get; set; } = "";

        public int TenantId { get; set; }

        public List<SubscriptionDto> Subscriptions { get; set; } = new List<SubscriptionDto>();
    }
}
