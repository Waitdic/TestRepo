namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Suppliers.Delete
{
    public record Request : IRequest<Response>
    {
        public int TenantId { get; set; }

        public int SubscriptionId { get; set; }

        public int SupplierId { get; set; }
    }
}
