namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Info
{
    public record Request : IRequest<Response>
    {
        public int TenantId { get; set; }

        public int SubscriptionId { get; set; }
    }
}
