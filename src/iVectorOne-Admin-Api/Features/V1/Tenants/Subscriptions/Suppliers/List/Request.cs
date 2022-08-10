namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Suppliers.List
{
    public record Request : IRequest<Response>
    {
        public int TenantId { get; set; }

        public int SubscriptionId { get; set; }
    }
}
