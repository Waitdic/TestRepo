namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.List
{
    public record Request : IRequest<Response>
    {
        public int TenantId { get; set; }

        public int AccountId { get; set; }
    }
}