namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Create
{
    public record Request : IRequest<Response>
    {
        public int TenantId { get; set; }
        public SubscriptionDto Subscription { get; set; } = null!;
    }
}
