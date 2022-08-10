namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Suppliers.List
{
    public record SubscriptionDto
    {
        public int SubscriptionId { get; set; }

        public List<SupplierDto> SupplierSubscriptions { get; set; } = new List<SupplierDto>();
    }
}
