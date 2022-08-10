namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Suppliers.List
{
    public record SupplierDto
    {
        public int SupplierSubscriptionID { get; set; }

        public int SupplierID { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool Enabled { get; set; } = false;
    }
}
