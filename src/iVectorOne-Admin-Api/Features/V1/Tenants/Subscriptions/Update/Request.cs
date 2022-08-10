namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Update
{
    public record Request : IRequest<Response>
    {
        public int TenantId { get; set; }
        public int SubscriptionId { get; set; }
        public SubscriptionDto Subscription { get; set; } = null!;
    }
}
