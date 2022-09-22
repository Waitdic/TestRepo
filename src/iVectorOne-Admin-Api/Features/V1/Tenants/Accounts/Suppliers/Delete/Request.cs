namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Delete
{
    public record Request : IRequest<Response>
    {
        public int TenantId { get; set; }

        public int AccountId { get; set; }

        public int SupplierId { get; set; }
    }
}